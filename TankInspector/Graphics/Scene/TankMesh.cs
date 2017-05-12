using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using log4net;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Graphics.Frameworks;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics.Scene
{
	internal class TankMesh : SceneMeshBase
    {
	    private class VertexState
        {
            public VertexBuffer VertexBuffer { get; set; }
            public VertexDeclaration VertexDeclaration { get; set; }
            public int Stride { get; set; }
            public int Count { get; set; }

            public void ApplyState(Device device)
            {
                device.SetStreamSource(0, VertexBuffer, 0, Stride);
                device.VertexDeclaration = VertexDeclaration;
            }
        };

        public struct Triangle
        {
            public Vector3 V1, V2, V3;
        }

        public class MeshGroup
        {
            public ArmorGroup Armor { get; set; }

            public IList<Vertex> RawVertices { get; set; }

            public IList<int> RawIndices { get; set; }

            public int StartIndex { get; set; }

            public int PrimitiveCount { get; set; }

            public IEnumerable<Triangle> Triangles
            {
                get
                {
                    for (int i = 0; i != PrimitiveCount; ++i)
                    {
                        int index = i * 3 + StartIndex;

                        yield return new Triangle
                        {
                                V1 = DxUtils.Convert(RawVertices[RawIndices[index]].Position),
                                V2 = DxUtils.Convert(RawVertices[RawIndices[index + 1]].Position),
                                V3 = DxUtils.Convert(RawVertices[RawIndices[index + 2]].Position),
                            };
                    }
                    yield break;
                }
            }
        }

	    private class RenderGroup : MeshGroup
        {
            public VertexState VertexState { get; set; }
            public IndexBuffer Indices { get; set; }


            public void ApplyState(Device device, Effect effect)
            {
                VertexState.ApplyState(device);
                device.Indices = Indices;

                if (RenderArmor)
                {
                    effect.SetValue("useDiffuse", false);
                    effect.SetValue("useNormal", false);
                    effect.SetValue("useSpecular", false);
                    effect.SetValue("useArmor", true);
                    effect.SetValue("spacingArmor", Armor.IsSpacingArmor);
                    effect.SetValue("armorValue", (float)Armor.Value);
                    effect.SetValue("armorChanceToHitByProjectile", (float)Armor.ChanceToHitByProjectile);

                }
                else
                {
                    BindTexture(device, effect);
                    effect.SetValue("useArmor", false);
                }
            }

            private void BindTexture(Device device, Effect effect)
            {
                if (Textures.ContainsKey("diffuseMap"))
                {
                    device.SetTexture(0, Textures["diffuseMap"].Texture);
                    effect.SetValue("useDiffuse", true);
                }
                else
                {
                    effect.SetValue("useDiffuse", false);
                }

                if (Textures.ContainsKey("normalMap") && ApplicationSettings.Default.NormalTextureEnabled)
                {
                    device.SetTexture(1, Textures["normalMap"].Texture);
                    effect.SetValue("useNormal", true);
                }
                else
                {
                    effect.SetValue("useNormal", false);
                }

                if (ApplicationSettings.Default.SpecularTextureEnabled)
                {
                    if(Textures.ContainsKey("specularMap"))
                    {
                        device.SetTexture(2, Textures["specularMap"].Texture);
                        effect.SetValue("useSpecular", true);
                        effect.SetValue("useMetallicDetail", false);

                    }
                    else if (Textures.ContainsKey("metallicGlossMap"))
                    {
                        device.SetTexture(2, Textures["metallicGlossMap"].Texture);
                        effect.SetValue("useSpecular", true);
                        if (Textures.ContainsKey("metallicDetailMap") && DetailPower > 0.0f)
                        {
                            device.SetTexture(3, Textures["metallicDetailMap"].Texture);
                            effect.SetValue("useMetallicDetail", true);
                            effect.SetValue("detailUVTiling", this.DetailUvTiling);
                            effect.SetValue("detailPower", DetailPower);
                        }
                    }
                    
                }
                else
                {
                    effect.SetValue("useSpecular", false);
                }


                effect.SetValue("alphaTestEnable", AlphaTestEnable);

                if (AlphaTestEnable)
                    effect.SetValue("alphaReference", (float) AlphaReference/255.0f);

                effect.SetValue("useNormalPackDXT1", this.UseNormalPackDxt1);

                effect.SetValue("useNormalBC1", this.UseNormalBc1);
            }

            public int VerticesCount { get; set; }

            public int MinVertexIndex { get; set; }

            public IDictionary<string, ModelTexture> Textures { get; set; }

            public bool AlphaTestEnable { get; set; }

            public bool UseNormalPackDxt1 { get; set; }

            public float DetailPower { get; set; }

            public Vector4 DetailUvTiling { get; set; }

            public int AlphaReference { get; set; }

            public bool RenderArmor { get; set; }

            public bool UseNormalBc1 { get; set; }

            public RenderGroup()
            {
                this.UseNormalPackDxt1 = true;
            }


        };

	    private class SubTankMesh
        {
            private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

            public IEnumerable<MeshGroup> Groups => _renderGroups;

	        private readonly List<RenderGroup> _renderGroups = new List<RenderGroup>();
            private static ModelTextureManager _textureManager;

            public void Render(Effect effect, ref int triangleCount,Device device)
            {
                if (Monitor.TryEnter(this))
                {
                    try
                    {
                        device.SetRenderState(RenderState.MultisampleAntialias, true);

                        foreach (var group in _renderGroups)
                        {
                            group.ApplyState(device, effect);

                            triangleCount += group.PrimitiveCount;

                            //device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

                            var a = group.Triangles;

                            effect.Begin();
                            effect.BeginPass(0);
                            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, group.MinVertexIndex, group.VerticesCount, group.StartIndex, group.PrimitiveCount);
                            effect.EndPass();
                            effect.End();
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
            }

            public void Dispose(bool disposing)
            {
                Monitor.Enter(this);
                try
                {
                    if (disposing)
                    {
                        foreach (var group in _renderGroups)
                        {
                            group.Indices.Dispose();
                            group.VertexState.VertexBuffer.Dispose();
                            group.VertexState.VertexDeclaration.Dispose();

                            if (group.Textures != null)
                            {
                                foreach (var texture in group.Textures)
                                {
                                    _textureManager.UnloadTexture(texture.Value);
                                }
                            }
                        }
                    }
                    _renderGroups.Clear();
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }

            internal static SubTankMesh FromModel(ModelVisual modelVisual, ModelPrimitive modelPrimitive , IPackageIndexer packageIndexer ,Device device, ModelTextureManager textureManager)
            {
                if ( modelVisual == null)
                    return null;
                if ( modelPrimitive == null)
                    return null;

                var mesh = new SubTankMesh();

                _textureManager = textureManager;


                var verticesMap = new Dictionary<string, VertexState>();


                foreach (var kv in modelPrimitive.Vertices)
                {
                    var name = kv.Key;
                    var vlist = kv.Value;

                    verticesMap[name] = TankMesh.ConvertToVertexBuffer(vlist, device);
                }


                var indicesMap = new Dictionary<string, IndexBuffer>();

                foreach (var kv in modelPrimitive.Indices)
                {
                    var name = kv.Key;
                    var nlist = kv.Value;

                    indicesMap[name] = TankMesh.ConvertToIndexBuffer(nlist, device);
                }



                foreach (var renderSet in modelVisual.RenderSets)
                {
                    //renderSet.Geometry.PrimitiveName
                    var vState = verticesMap[renderSet.Geometry.VerticesName];
                    var indices = indicesMap[renderSet.Geometry.IndicesName];
                    var rawVertices = modelPrimitive.Vertices[renderSet.Geometry.VerticesName].Vertices;
                    var rawIndices = modelPrimitive.Indices[renderSet.Geometry.IndicesName];



                    foreach (var groupKv in renderSet.Geometry.ModelPrimitiveGroups)
                    {
                        var group = groupKv.Value;

                        RenderGroup renderGroup = null;

                        if (group.Sectioned)
                        {
                            renderGroup = new RenderGroup
                            {
                                MinVertexIndex = (int)group.StartVertex,
                                VerticesCount = (int)group.VerticesCount,
                                StartIndex = (int)group.StartIndex,
                                PrimitiveCount = (int)group.PrimitiveCount,
                            };
                        }
                        else
                        {
                          renderGroup = new RenderGroup
                          {
                              MinVertexIndex = 0,
                              VerticesCount = vState.Count,
                              StartIndex = 0,
                              PrimitiveCount = ((int)indices.Tag) / 3,
                          };
                        }

                        renderGroup.VertexState = vState;
                        renderGroup.Indices = indices;
                        renderGroup.RawVertices = rawVertices;
                        renderGroup.RawIndices = rawIndices;

                        if (group.Material.ShowArmor)
                        {
                            renderGroup.RenderArmor = true;
                            renderGroup.Textures = null;
                            renderGroup.Armor = group.Material.Armor;

                        }
                        else
                        {
                            renderGroup.RenderArmor = false;

                            var textures = new Dictionary<string, ModelTexture>();

                            foreach (var property in group.Material.Propertys)
                            {
                                var texturePath = property.Texture;

                                if ( string.IsNullOrWhiteSpace(texturePath))
                                {
                                    if (property.Name == "alphaTestEnable" && group.Material.Fx != "shaders/std_effects/PBS_tank.fx")
                                    {
                                        renderGroup.AlphaTestEnable = property.BoolValue;
                                    }
                                    else
                                    {

                                        switch (property.Name)
                                        {
                                            case "alphaReference":
                                                renderGroup.AlphaReference = property.IntValue;
                                                break;
                                            case "g_useNormalPackDXT1":
                                                renderGroup.UseNormalPackDxt1 = property.BoolValue;
                                                break;
                                            case "g_detailPower":
                                                renderGroup.DetailPower = property.FloatValue;
                                                break;
                                            case "g_metallicDetailUVTiling":
                                                renderGroup.DetailUvTiling = property.Vector4Value;
                                                break;
                                            case "g_defaultPBSConversionParam":
                                                break;
                                            case "g_albedoConversions":
                                                break;
                                            case "g_glossConversions":
                                                break;
                                            case "g_metallicConversions":
                                                break;
                                            case "g_defaultPBSConversionParams":
                                                break;
                                            case "g_albedoCorrection":
                                                break;
                                            case "g_useDetailMetallic":
                                                break;
                                            case "g_maskBias":
                                                break;
                                            case "doubleSided":
                                                break;
                                            case "alphaTestEnable":
                                                break;
                                            case "crash_coefficient":
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        var texture = textureManager.LoadTexture(packageIndexer, texturePath, device);

                                        textures[property.Name] = texture;

                                        if (property.Name == "normalMap" && !renderGroup.UseNormalBc1)
                                        {
                                            if (texture.ImageInformation.Format == Format.Dxt1)
                                            {
                                                renderGroup.UseNormalBc1 = true;
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Log.InfoFormat("can't load texture {0} for {1}", texturePath, property.Name);
                                    }
                                }
                            }

                            renderGroup.Textures = textures;
                        }

                        mesh._renderGroups.Add(renderGroup);
                    }
                }
                return mesh;
            }

            private static PackageStream OpenTexture(IPackageIndexer packageIndexer, string path) 
            {
                var packagePath = packageIndexer.GetPackagePath(path);
                
                if (packagePath == null && Path.GetExtension(path) == ".tga")
                {
                    path = path.Substring(0, path.Length - 4) + ".dds";
                    packagePath = packageIndexer.GetPackagePath(path);
                }

                return new PackageStream(packagePath, path);
            }
        }

        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
        private Device _device;

        private SubTankMesh _undamagedMesh;
        private SubTankMesh _collisionMesh;

        public ITank Tank { get; private set; }


        public IEnumerable<MeshGroup> CollisionGroup 
        {
            get
            {
                if (_collisionMesh == null)
                    return null;

                return _collisionMesh.Groups;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_undamagedMesh != null)
            {
                _undamagedMesh.Dispose(disposing);
            }
            if (_undamagedMesh != null)
            {
                _collisionMesh.Dispose(disposing);
            }
            base.Dispose(disposing);
        }

        public void Render(Effect effect, ref int triangleCount , Model.ModelType type)
        {
            switch (type)
            {
            case Model.ModelType.Undamaged:
                    if (_undamagedMesh != null) 
                        _undamagedMesh.Render(effect, ref triangleCount, _device);
                    break;
            case Model.ModelType.Collision:
                    if (_collisionMesh != null) 
                        _collisionMesh.Render(effect, ref triangleCount, _device);
                    break;
            }
        }


        public static TankMesh FromModel(Model model, Device device,ModelTextureManager textureManager)
        {
            Log.InfoFormat("load tank mesh {0} from {1}", model.ModelName, ((ITankObject)model.TankObject).ShortName);

            var mesh = new TankMesh();
            mesh.Tank = model.TankObject;
            mesh._device = device;

            try
            {
                mesh._undamagedMesh = SubTankMesh.FromModel(model.Visual, model.Primitive, model.Database.PackageDatabase, device, textureManager);
            }
            catch (Exception e)
            {
                Log.Info("exception occurred when load undamaged mesh", e);
                mesh._undamagedMesh = null;
            }
            try
            {
                mesh._collisionMesh = SubTankMesh.FromModel(model.CollisionVisual, model.CollisionPrimitive, model.Database.PackageDatabase, device, textureManager);
            }
            catch (Exception e)
            {
                Log.Info("exception occurred when load collision mesh", e);
                mesh._collisionMesh = null;
            }
            return mesh;
        }

        private static IndexBuffer ConvertToIndexBuffer(IList<int> nlist, Device device)
        {
            var indicesBuffer = new IndexBuffer(device, sizeof(int) * nlist.Count, Usage.WriteOnly, Pool.Default, false);
            var data = indicesBuffer.Lock(0, 0, LockFlags.None);

            foreach (var n in nlist)
            {
                data.Write(n);
            }
            indicesBuffer.Unlock();
            indicesBuffer.Tag = nlist.Count;
            return indicesBuffer;
        }


        private static VertexState ConvertToVertexBuffer(ModelPrimitive.VerticesList vlist, Device device)
        {
            var vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        		new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                new VertexElement(0, 32, DeclarationType.Float3,DeclarationMethod.Default,DeclarationUsage.Tangent,0),
                new VertexElement(0, 44, DeclarationType.Float3,DeclarationMethod.Default,DeclarationUsage.Binormal,0),
				VertexElement.VertexDeclarationEnd
        	};

            var vertexSize = 56;

            var vertexBuffer = new VertexBuffer(device, vertexSize * vlist.Vertices.Count, Usage.WriteOnly, VertexFormat.None, Pool.Default);

            var data = vertexBuffer.Lock(0, 0, LockFlags.None);
            foreach (var v in vlist.Vertices)
            {
                data.Write(DxUtils.Convert(v.Position));
                data.Write(DxUtils.Convert(v.Normal));
                data.Write(DxUtils.Convert(v.TextureCoordinates));
                data.Write(DxUtils.Convert(v.Tangent));
                data.Write(DxUtils.Convert(v.Binormal));
            }

            var vertexDecl = new VertexDeclaration(device, vertexElems);

            vertexBuffer.Unlock();

            return new VertexState { VertexBuffer = vertexBuffer, VertexDeclaration = vertexDecl, Stride = vertexSize, Count = vlist.Vertices.Count };
        }
    }
}
