using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;
using log4net;
using System.Reflection;

namespace Smellyriver.TankInspector.Graphics
{
    public struct PrimitiveSection
    {
        public string Name { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
    }
    public static class VertexFormatExtension
    {
        public static bool IsSkinned(this ModelPrimitive.VertexFormat vf)
        {
            return vf == ModelPrimitive.VertexFormat.Xyznuviiiwwtb || vf == ModelPrimitive.VertexFormat.Set3Xyznuviiiwwtbpc;
        }
    }

    public class ModelPrimitive
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        static private Vector3D UnpackNormal(UInt32 packed)
        {
            var pkz = (packed >> 22) & 0x3FF;

            var pky = (packed >> 11) & 0x7FF;

            var pkx = (packed) & 0x7FF;
            float x, y, z;
            if (pkx > 0x3ff)
                x = -(float)((pkx & 0x3ff ^ 0x3ff) + 1) / 0x3ff;
            else
                x = (float)(pkx) / 0x3ff;

            if (pky > 0x3ff)
                y = -(float)((pky & 0x3ff ^ 0x3ff) + 1) / 0x3ff;
            else
                y = (float)(pky) / 0x3ff;

            if (pkz > 0x1ff)
                z = -(float)((pkz & 0x1ff ^ 0x1ff) + 1) / 0x1ff;

            else
                z = (float)(pkz) / 0x1ff;

            return new Vector3D(x, y, z);
        }

        static private Vector3D UnpackNormal_tag3(UInt32 packed)
        {
            var pkz = (packed >> 16) & 0xFF ^ 0xFF;

            var pky = (packed >> 8) & 0xFF ^ 0xFF;

            var pkx = (packed) & 0xFF ^ 0xFF;
            float x, y, z;

            if (pkx > 0x7f)
                x = -(float)(pkx & 0x7f) / 0x7f;
            else
                x = (float)(pkx ^ 0x7f) / 0x7f;

            if (pky > 0x7f)
                y = -(float)(pky & 0x7f) / 0x7f;
            else
		        y = (float)(pky ^ 0x7f) / 0x7f;

            if (pkz > 0x7f)
		        z = -(float)(pkz & 0x7f) / 0x7f;
	        else
		        z = (float)(pkz ^ 0x7f) / 0x7f;

            return new Vector3D(x, y, z);
        }




        public class VerticesList
        {
            public IList<Vertex> Vertices { get; set; }
            public VertexFormat Format { get; set; }
            public bool RecalculateNormals { get; set; }
        }


        public enum VertexFormat
        {
            Xyznuvtb,
            Xyznuviiiwwtb,
            Xyznuv,
            Set3Xyznuviiiwwtbpc,
            Set3Xyznuvtbpc,
            Set3Xyznuvpc,
        }
            
        public Dictionary<string, VerticesList> Vertices { get; }

        public Dictionary<string, IList<int>> Indices { get; }

	    private ModelPrimitive()
        {
            Vertices = new Dictionary<string, VerticesList>();
            Indices = new Dictionary<string, IList<int>>();
        }

        internal static ModelPrimitive ReadFrom(Stream stream, ModelVisual visual, bool recalculateNormals)
        {
            var primitive = new ModelPrimitive();
            var sections = ModelPrimitive.ReadSections(stream);


            foreach (var section in sections)
            {
                var renderSets = visual.RenderSets.FindAll(s => s.Geometry.VerticesName == section.Name);
                if (renderSets.Count != 0)
                {
                    var vertices = ModelPrimitive.ReadVerticesFrom(stream, section, recalculateNormals);
                    primitive.Vertices.Add(section.Name, vertices);

#if WITHOUT_D3D
                    var positions = new Point3DCollection(vertices.Vertices.Select(v => v.Position));
                    var textureCoordinates = new PointCollection(vertices.Vertices.Select(v => v.TextureCoordinates));
                    var normals = new Vector3DCollection(vertices.Vertices.Select(v => v.Normal));

                    positions.Freeze();
                    textureCoordinates.Freeze();
                    normals.Freeze();

                    foreach (var renderSet in renderSets)
                    {
                        renderSet.Geometry.Positions = positions;
                        renderSet.Geometry.TextureCoordinates = textureCoordinates;
                        renderSet.Geometry.Normals = normals;
                    }
#endif

                }
                else
                {
                    renderSets = visual.RenderSets.FindAll(s => s.Geometry.IndicesName == section.Name);

                    if (renderSets.Count != 0)
                    {
                        var indices = ModelPrimitive.ReadIndicesFrom(stream, section, renderSets);
                        primitive.Indices.Add(section.Name, indices);
#if WITHOUT_D3D
                        var triangleIndices = new Int32Collection(indices);
                        triangleIndices.Freeze();
                        foreach (var renderSet in renderSets)
                        {
                            renderSet.Geometry.TriangleIndices = triangleIndices;
                        }
#endif
                    }
                }
            }

            if (recalculateNormals)
            {
                foreach (var renderSet in visual.RenderSets)
                {
                    var verticesList = primitive.Vertices[renderSet.Geometry.VerticesName];
                    if (verticesList.RecalculateNormals)
                        continue;

                    var vertices = verticesList.Vertices;
                    var indices = primitive.Indices[renderSet.Geometry.IndicesName];

                    for (int i = 0; i != indices.Count; i += 3)
                    {
                        var index0 = indices[i];
                        var index1 = indices[i + 1];
                        var index2 = indices[i + 2];

                        var normal = ModelPrimitive.CalculateNormal(vertices[index0].Position, vertices[index1].Position, vertices[index2].Position);

                        //if (verticesList.Format == VertexFormat.xyznuviiiwwtb)
                        //{
                        //    normal.Z = -normal.Z;
                        //}

                        vertices[index0].Normal += normal;
                        vertices[index1].Normal += normal;
                        vertices[index2].Normal += normal;

                    }

                    //foreach (var v in vertices)
                    //{
                    //    v.Normal.Normalize();
                    //}

                    verticesList.RecalculateNormals = true;
                }
            }

            return primitive;
        }

        private static Vector3D CalculateNormal(Point3D p1, Point3D p2, Point3D p3)
        {
            double w0, w1, w2, v0, v1, v2, nx, ny, nz;
            w0 = p2.X - p1.X; w1 = p2.Y - p1.Y; w2 = p2.Z - p1.Z;
            v0 = p3.X - p1.X; v1 = p3.Y - p1.Y; v2 = p3.Z - p1.Z;
            nx = (w1 * v2 - w2 * v1);
            ny = (w2 * v0 - w0 * v2);
            nz = (w0 * v1 - w1 * v0);
            return new Vector3D(nx, ny, nz);
        }

        private static IList<int> ReadIndicesFrom(Stream stream, PrimitiveSection section,IList<ModelRenderSet> renderSets)
        {
            //Contract.Assert(section.Name == "indices");

            var reader = new BinaryReader(stream);

            reader.BaseStream.Seek(section.Offset, SeekOrigin.Begin);

            string indexSizeType = "";

            bool hasZero = false;
            for (int i = 0; i != 64; ++i)
            {
                int @byte = reader.ReadByte();
                if (@byte == 0)
                {
                    hasZero = true;
                }
                if (((@byte > 0x40) & (@byte <= 0x7b)) && !hasZero)
                {
                    indexSizeType += (char)@byte;
                }
            }
            int indexSize;
            if (indexSizeType == "list")
            {
                indexSize = 2;
            }
            else if (indexSizeType == "list32")
            {
                indexSize = 4;
            }
            else
            {
                throw new InvalidDataException(
	                $"this index type is {indexSizeType},only support list and list32 index type ");
            }


            var indicesCount = reader.ReadUInt32();
            var groups = reader.ReadUInt32();

            var indices = new List<int>((int)indicesCount);
            for (int i = 0; i != indicesCount; ++i)
            {
                if (indexSize == 2)
                {
                    indices.Add((int)reader.ReadUInt16());
                }
                else
                {
                    indices.Add((int)reader.ReadUInt32());
                }
            }

            for (int i = 0; i != groups; ++i)
            {
                var startIndex = reader.ReadUInt32();
                var nPrimitives = reader.ReadUInt32();
                var startVertex = reader.ReadUInt32();
                var nVertices = reader.ReadUInt32();
                foreach (var renderSet in renderSets)
                {
					if (renderSet.Geometry.ModelPrimitiveGroups.TryGetValue(i, out ModelPrimitiveGroup group))
					{
						group.StartIndex = startIndex;
						group.PrimitiveCount = nPrimitives;
						group.EndIndex = startIndex + nPrimitives;
						group.StartVertex = startVertex;
						group.VerticesCount = nVertices;
						group.EndVertex = startVertex + nVertices;
						group.Sectioned = true;
					}
				}
            }

            return indices;
        }



        private static unsafe VerticesList ReadVerticesFrom(Stream stream, PrimitiveSection section, bool recalculateNormals)
        {
            //Contract.Assert(section.Name == "vertices");

            var reader = new BinaryReader(stream);

            reader.BaseStream.Seek(section.Offset, SeekOrigin.Begin);

            var pbvt = reader.ReadInt32();

            var newVersion = pbvt == 0x54565042;
            // not new version seek to begin.
            if (!newVersion)
            {
                reader.BaseStream.Seek(section.Offset, SeekOrigin.Begin);
            }
            else
            {
                reader.BaseStream.Seek(section.Offset + 68, SeekOrigin.Begin);
            }

            string vertexFormat = "";

            bool hasZero = false;
            for (int i = 0; i != 64; ++i)
            {
                int @byte = reader.ReadByte();
                if (@byte == 0)
                {
                    hasZero = true;
                }
                if (((@byte > 0x40) & (@byte <= 0x7b)) && !hasZero)
                {
                    vertexFormat += (char)@byte;
                }
            }

            VertexFormat vf;

            if (vertexFormat == "set3/xyznuviiiwwtbpc" || vertexFormat== "setxyznuviiiwwtbpc")
            {
                vf = VertexFormat.Set3Xyznuviiiwwtbpc;
                //SIZE = 40
                //UNPACK_FORMAT = '<3fI2f8B2I'
                //is_skinned = True
            }
            else if(vertexFormat == "set3/xyznuvtbpc" || vertexFormat == "setxyznuvtbpc")
            {
                //    SIZE = 32
                //UNPACK_FORMAT = '<3fI2f2I'
                vf = VertexFormat.Set3Xyznuvtbpc;
            }
            else if(vertexFormat == "set3/xyznuvpc" || vertexFormat == "setxyznuvpc")
            {
                //    SIZE = 24
                //UNPACK_FORMAT = '<3fI2f'
                vf = VertexFormat.Set3Xyznuvpc;
            }
            else if (vertexFormat.Contains( "xyznuviiiwwtb"))
            {
                //SIZE = 37
                //UNPACK_FORMAT = '<3fI2f5B2I'
                //is_skinned = True
                vf = VertexFormat.Xyznuviiiwwtb;
            }
            else if (vertexFormat.Contains("xyznuvtb"))
            {
                //SIZE = 32
                //UNPACK_FORMAT = '<3fI2f2I'
                vf = VertexFormat.Xyznuvtb;
            }
            else if (vertexFormat.Contains("xyznuv"))
            {
                //SIZE = 24
                //UNPACK_FORMAT = '<3fI2f'
                vf = VertexFormat.Xyznuv;
            }
            else
            {
                throw new FormatException("Unexcpeted Vertex Format.");
            }

            var vertexCount = reader.ReadUInt32();

            var vertices = new List<Vertex>((int)vertexCount);

            for (int i = 0; i != vertexCount; ++i)
            {
                var v = new Vertex();

                // 3fI2f8B2I

                v.Position.X = reader.ReadSingle();
                v.Position.Y = reader.ReadSingle();
                v.Position.Z = reader.ReadSingle() * (vf.IsSkinned() ? -1f : 1f);

                var normal = reader.ReadUInt32();
                if (!recalculateNormals)
                {
                    v.Normal = newVersion ? ModelPrimitive.UnpackNormal_tag3(normal) : ModelPrimitive.UnpackNormal(normal);
                }


                v.TextureCoordinates.X = reader.ReadSingle();
                v.TextureCoordinates.Y = reader.ReadSingle();

                if (vf.IsSkinned())
                {

                    v.Index1 = reader.ReadByte();
                    v.Index2 = reader.ReadByte();
                    v.Index3 = reader.ReadByte();

                    if (vf == VertexFormat.Set3Xyznuviiiwwtbpc)
                    {
                        reader.ReadBytes(3);//indexb1 b2 b3
                    }

                    v.Weight1 = reader.ReadByte();
                    v.Weight2 = reader.ReadByte();
                }

                if (vf != VertexFormat.Xyznuv && vf != VertexFormat.Set3Xyznuvpc)
                {

                    if (!recalculateNormals)
                    {
                        var tangent = reader.ReadUInt32();
                        var binormal = reader.ReadUInt32();
                        v.Tangent = newVersion ? ModelPrimitive.UnpackNormal_tag3(tangent) : ModelPrimitive.UnpackNormal(tangent);
                        v.Binormal = newVersion ? ModelPrimitive.UnpackNormal_tag3(binormal) : ModelPrimitive.UnpackNormal(binormal);
                        if (vf.IsSkinned())
                        {
                            //v.Tangent.Z = -v.Tangent.Z;
                            //v.Binormal.Z = -v.Binormal.Z;
                            v.Normal.Z = -v.Normal.Z;
                        }
                    }
                    else
                    {
                        reader.ReadUInt32();
                        reader.ReadUInt32();
                    }
                }

                vertices.Add(v);
            }

            return new VerticesList { Vertices = vertices, Format = vf, RecalculateNormals = false };
        }


        private static List<PrimitiveSection> ReadSections(Stream stream)
        {
            var sections = new List<PrimitiveSection>();

            long dataOffset = 4;

            var reader = new BinaryReader(stream);
            reader.BaseStream.Seek(-4, SeekOrigin.End);

            var infoOffset = reader.ReadInt32();

            reader.BaseStream.Seek(-(infoOffset + 4), SeekOrigin.Current);

            while (reader.BaseStream.Length - reader.BaseStream.Position > 8)
            {
                //对齐到下个边界，开源算法有BUG
                reader.BaseStream.Position = (reader.BaseStream.Position + 3) & ~3L;

                var dataLength = 0;

                while (dataLength == 0)
                    dataLength = reader.ReadInt32();

                var sectionNameLength = 0;

                while (sectionNameLength == 0)
                    sectionNameLength = reader.ReadInt32();

                string sectionName = new string(reader.ReadChars(sectionNameLength));

                sections.Add(new PrimitiveSection { Name = sectionName, Offset = dataOffset, Length = dataLength });

                dataOffset += (dataLength + 3) & (~3L);
            }
            return sections;
        }

#if false
        private unsafe void build_primitive_data(bool _add)
        {
            bool flag;
            BinaryReader reader;
            uint num;
            byte[] buffer;
            byte num2;
            uint num3;
            string str;
            uint num4;
            int num5;
            int num6;
            string str2;
            string str3;
            string str4;
            string str5;reader
            ulong num7;
            string str6;
            uint num8;
            uint num9;
            int num10;
            int num11;
            int num12;
            string str7;
            Array array;
            string str8;
            bool flag2;
            Globals.primGroup[] groupArray;
            long num13;
            FileStream stream;
            Array array2;
            long num14;
            ulong[] numArray;
            uint[] numArray2;
            MemoryStream stream2;
            Globals.VertexXYZNUVTB xxyznuvtb;
            int num15;
            Classes.vertice_[] e_Array;
            string str9;
            int num16;
            int num17;
            int num18;
            uint num19;
            uint num20;
            BinaryWriter writer;
            FileStream stream3;
            long num21;
            long num22;
            uint num23;
            int num24;
            byte num25;
            bool flag3;
            BinaryReader reader2;
            int num26;
            uint num27;
            int num28;
            int num29;
            uint num30;
            uint num31;
            uint num32;
            BinaryReader reader3;
            FileStream stream4;
            uint num33;
            uint num34;
            DataRow row;
            int num35;
            uint num36;
            uint num37;
            uint num38;
            Globals.vect3 vect;
            Exception exception;
            int num39;
            int num40;
            uint num41;
            uint num42;
            uint num43;
            uint num44;
            uint num45;
            uint num46;
            object[] objArray;
            uint num47;
            uint num48;
            uint num49;
            uint num50;
            uint num51;
            uint num52;
            uint num53;
            char[] chArray;
            int num54;
            flag2 = 0;
            Globals.has_uv2 = 0;
            Globals.has_color = 0;
            num40 = 0;
            Il.ilDeleteImages(100, &num40);
            num5 = Gl.glGetError();
            if (_add != null)
            {
                goto Label_004C;
            }
            num8 = 1;
        Label_0028:
            Gl.glDeleteTextures(1, &num8);
            this.texID[(int)(((ulong)num8) - 1L)] = num8;
            num8 += 1;
            if (num8 <= 100)
            {
                goto Label_0028;
            }
        Label_004C:
            num5 = Gl.glGetError();
            Console.WriteLine(Marshal.SizeOf((Globals.VertexXYZNUVTB)xxyznuvtb));
            numArray = new ulong[0x65];
            numArray2 = new uint[0x65];
            groupArray = new Globals.primGroup[2];
            this.DGV_objs.ClearSelection();
            if (_add != null)
            {
                goto Label_0132;
            }
            if (this.DGV_objs.Rows.Count <= 0)
            {
                goto Label_00DB;
            }
            num16 = this.DGV_objs.Rows.Count;
            num41 = (uint)num16;
            num8 = 1;
            goto Label_00D5;
        Label_00BE:
            this.DGV_objs.Rows.RemoveAt(0);
            num8 += 1;
        Label_00D5:
            if (num8 <= num41)
            {
                goto Label_00BE;
            }
        Label_00DB:
            this.DGV.ClearSelection();
            if (this.DGV.Rows.Count <= 0)
            {
                goto Label_0132;
            }
            num17 = this.DGV.Rows.Count;
            num42 = (uint)num17;
            num8 = 1;
            goto Label_012C;
        Label_0115:
            this.DGV.Rows.RemoveAt(0);
            num8 += 1;
        Label_012C:
            if (num8 <= num42)
            {
                goto Label_0115;
            }
        Label_0132:
            Globals.normals = (Classes.norm[])Utils.CopyArray((Array)Globals.normals, new Classes.norm[0xfde9]);
            Globals.tris = (Classes.triangle[])Utils.CopyArray((Array)Globals.tris, new Classes.triangle[0xfde9]);
            Globals.vertex = (Classes.verts[])Utils.CopyArray((Array)Globals.vertex, new Classes.verts[0xfde9]);
            Globals.faces = (Classes.obj[])Utils.CopyArray((Array)Globals.faces, new Classes.obj[2]);
            Globals.faces[0] = new Classes.obj();
            Globals.new_obj = (Classes.obj[])Utils.CopyArray((Array)Globals.new_obj, new Classes.obj[0xfde9]);
            num8 = 1;
        Label_01EC:
            Globals.normals[(int)num8] = new Classes.norm();
            Globals.tris[(int)num8] = new Classes.triangle();
            Globals.vertex[(int)num8] = new Classes.verts();
            Globals.new_obj[(int)num8] = new Classes.obj();
            num8 += 1;
            if (num8 <= 0xfde8)
            {
                goto Label_01EC;
            }
            if (_add != null)
            {
                goto Label_0280;
            }
            Globals.x_max = -10000f;
            Globals.x_min = 10000f;
            Globals.y_max = -10000f;
            Globals.y_min = 10000f;
            Globals.z_max = -10000f;
            Globals.z_min = 10000f;
            Globals.master_cnt = 0;
            Globals.object_start = 1;
            goto Label_0295;
        Label_0280:
            Globals.master_cnt = Convert.ToUInt32(this.poly_count.Text);
        Label_0295:
            str = "";
            num11 = 0;
            num6 = 0;
            num3 = 0;
            str7 = "";
            str6 = "";
            this.TextBox2.Text = this.TextBox2.Text.Replace("_back", "");
            str6 = this.TextBox2.Text.Replace(".primitives", ".visual");
            Globals.xmlget_mode = 0;
            if (Strings.InStr(str6, "Hull", 0) <= 0)
            {
                goto Label_0314;
            }
            Globals.xmlget_mode = 1;
        Label_0314:
            if (Strings.InStr(str6, "Chassis", 0) <= 0)
            {
                goto Label_032A;
            }
            Globals.xmlget_mode = 2;
        Label_032A:
            if (Strings.InStr(str6, "Turret", 0) <= 0)
            {
                goto Label_0340;
            }
            Globals.xmlget_mode = 3;
        Label_0340:
            if (Strings.InStr(str6, "Gun", 0) <= 0)
            {
                goto Label_0356;
            }
            Globals.xmlget_mode = 4;
        Label_0356:
            if (((Globals.xmlget_mode == 2) | (Globals.xmlget_mode == 4)) == null)
            {
                goto Label_0371;
            }
            Globals.has_bsp = 0;
            goto Label_0377;
        Label_0371:
            Globals.has_bsp = 1;
        Label_0377:
            vis_main.openVisual(str6);
        Label_037F:
            try
            {
                stream = new FileStream(this.TextBox2.Text, 3, 1);
                goto Label_03B6;
            }
            catch (Exception exception1)
            {
            Label_0395:
                ProjectData.SetProjectError(exception1);
                Interaction.MsgBox("File Not Found!", 0x30, "IO Error");
                ProjectData.ClearProjectError();
                goto Label_22B1;
            }
        Label_03B6:
            reader = new BinaryReader(stream);
            num7 = (ulong)stream.Length;
            buffer = new byte[((int)num7) + 1];
            num43 = Convert.ToUInt32(decimal.Subtract(new decimal(num7), decimal.One));
            num8 = 0;
            goto Label_0401;
        Label_03F0:
            buffer[(int)num8] = reader.ReadByte();
            num8 += 1;
        Label_0401:
            if (num8 <= num43)
            {
                goto Label_03F0;
            }
            stream.Dispose();
            stream2 = new MemoryStream(buffer);
            reader = new BinaryReader(stream2);
            reader.BaseStream.Seek(Convert.ToInt64(decimal.Subtract(new decimal(num7), new decimal(4L))), 0);
            num13 = reader.BaseStream.Position;
            num14 = (long)reader.ReadInt32();
            reader.BaseStream.Seek(Convert.ToInt64(decimal.Subtract(decimal.Subtract(new decimal(num7), new decimal(4L)), new decimal(num14))), 0);
            Globals.section_names = (string[])Utils.CopyArray((Array)Globals.section_names, new string[0x65]);
            num8 = 0;
        Label_04AD:
            if (decimal.Compare(new decimal(reader.BaseStream.Position), decimal.Subtract(new decimal(num7), new decimal(4L))) >= 0)
            {
                goto Label_0593;
            }
            numArray2[(int)num8] = reader.ReadUInt32();
            num4 = reader.ReadUInt32();
            num4 = reader.ReadUInt32();
            num4 = reader.ReadUInt32();
            num4 = reader.ReadUInt32();
            num19 = reader.ReadUInt32();
            num44 = num19;
            num20 = 1;
            goto Label_0532;
        Label_0518:
            str7 = str7 + Conversions.ToString(reader.ReadChar());
            num20 += 1;
        Label_0532:
            if (num20 <= num44)
            {
                goto Label_0518;
            }
            Globals.section_names[(int)num8] = str7;
            if (Strings.InStr(str7, "colour", 0) <= 0)
            {
                goto Label_0559;
            }
            Globals.has_color = 1;
        Label_0559:
            if (Strings.InStr(str7, "uv2", 0) <= 0)
            {
                goto Label_056F;
            }
            Globals.has_uv2 = 1;
        Label_056F:
            num18 = str7.Length % 4;
            if (num18 <= 0)
            {
                goto Label_058A;
            }
            reader.ReadChars(4 - num18);
        Label_058A:
            str7 = "";
            goto Label_05DD;
        Label_0593:
            numArray2 = (uint[])Utils.CopyArray((Array)numArray2, new uint[((int)num8) + 1]);
            Globals.section_names = (string[])Utils.CopyArray((Array)Globals.section_names, new string[((int)num8) + 1]);
            Globals.number_of_groups = (int)num8;
            goto Label_05EC;
        Label_05DD:
            num8 += 1;
            if (num8 <= 0x63)
            {
                goto Label_04AD;
            }
        Label_05EC:
            num2 = 0;
            reader.BaseStream.Seek(0L, 0);
            num4 = reader.ReadUInt32();
            str5 = "";
            str3 = "";
            str4 = "";
            if (Directory.Exists(@"C:\wot_temp\") != null)
            {
                goto Label_0632;
            }
            Directory.CreateDirectory(@"C:\wot_temp\");
        Label_0632:
            num15 = 0;
            array = vis_main.get_section_names();
            num45 = (uint)(Globals.number_of_groups - 1);
            num8 = 0;
            goto Label_07B6;
        Label_064E:
            if (Strings.InStr(Globals.section_names[(int)num8], "vertices", 0) <= 0)
            {
                goto Label_0685;
            }
            str5 = @"C:\wot_temp\" + Globals.section_names[(int)num8] + ".sec";
            num15 += 1;
        Label_0685:
            if (Strings.InStr(Globals.section_names[(int)num8], "indices", 0) <= 0)
            {
                goto Label_06B6;
            }
            str3 = @"C:\wot_temp\" + Globals.section_names[(int)num8] + ".sec";
        Label_06B6:
            if (Strings.InStr(Globals.section_names[(int)num8], "uv2", 0) <= 0)
            {
                goto Label_06E7;
            }
            str4 = @"C:\wot_temp\" + Globals.section_names[(int)num8] + ".sec";
        Label_06E7:
            if (Strings.InStr(Globals.section_names[(int)num8], "colour", 0) <= 0)
            {
                goto Label_0718;
            }
            str2 = @"C:\wot_temp\" + Globals.section_names[(int)num8] + ".sec";
        Label_0718:
            stream3 = new FileStream(@"C:\WoT_Temp\" + Globals.section_names[(int)num8] + ".sec", 2, 2);
            writer = new BinaryWriter(stream3);
            num22 = reader.BaseStream.Position;
            num46 = numArray2[(int)num8];
            num23 = 1;
            goto Label_0773;
        Label_075C:
            num2 = reader.ReadByte();
            writer.Write(num2);
            num23 += 1;
        Label_0773:
            if (num23 <= num46)
            {
                goto Label_075C;
            }
            writer.Close();
            stream3.Close();
            stream3.Dispose();
            stream3 = null;
            num21 = ((ulong)numArray2[(int)num8]) % 4L;
            if (num21 <= 0L)
            {
                goto Label_07B0;
            }
            reader.ReadChars((int)(4L - num21));
        Label_07B0:
            num8 += 1;
        Label_07B6:
            if (num8 <= num45)
            {
                goto Label_064E;
            }
            reader.Close();
            buffer = null;
            num12 = 0;
            flag = _add;
        Label_07CC:
            try
            {
                num24 = num15 - 1;
                goto Label_2116;
            Label_07D7:
                num15 -= 1;
                str5 = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(@"C:\wot_temp\", NewLateBinding.LateIndexGet(array, new object[] { (int)((num24 - num15) * 2) }, null)), ".sec"));
                str3 = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(@"C:\wot_temp\", NewLateBinding.LateIndexGet(array, new object[] { (int)(((num24 - num15) * 2) + 1) }, null)), ".sec"));
                if (num15 <= 0)
                {
                    goto Label_0861;
                }
                flag2 = 1;
            Label_0861:
                stream = new FileStream(str3, 3, 1);
                reader2 = new BinaryReader(stream);
                reader2.BaseStream.Seek(0L, 0);
                flag3 = 0;
                num8 = 0;
            Label_088B:
                num25 = reader2.ReadByte();
                if (num25 != 0)
                {
                    goto Label_089C;
                }
                flag3 = 1;
            Label_089C:
                if (((num25 > 30) & ((num25 > 0x7b) == 0)) == null)
                {
                    goto Label_08C7;
                }
                if (flag3 != null)
                {
                    goto Label_08C7;
                }
                str7 = str7 + Conversions.ToString(Strings.Chr(num25));
            Label_08C7:
                num8 += 1;
                if (num8 <= 0x3f)
                {
                    goto Label_088B;
                }
                num30 = 0;
                &this.ih.ind_h = str7;
                if (Strings.InStr(str7, "list32", 0) <= 0)
                {
                    goto Label_08FB;
                }
                Globals.ind_scale = 4;
                goto Label_0901;
            Label_08FB:
                Globals.ind_scale = 2;
            Label_0901:
                &this.ih.nIndices_ = reader2.ReadUInt32();
                &this.ih.nInd_groups = reader2.ReadUInt16();
                flag3 = 0;
                str7 = "";
                groupArray = new Globals.primGroup[&this.ih.nInd_groups + 1];
                num27 = (uint)(((ulong)(&this.ih.nIndices_ * Globals.ind_scale)) + 0x48L);
                reader2.BaseStream.Seek((ulong)num27, 0);
                num47 = (uint)(&this.ih.nInd_groups - 1);
                num8 = 0;
                goto Label_09E4;
            Label_0982:
                &(groupArray[(int)num8]).startIndex_ = (ulong)reader2.ReadUInt32();
                &(groupArray[(int)num8]).nPrimitives_ = (ulong)reader2.ReadUInt32();
                &(groupArray[(int)num8]).startVertex_ = (ulong)reader2.ReadUInt32();
                &(groupArray[(int)num8]).nVertices_ = (ulong)reader2.ReadUInt32();
                num8 += 1;
            Label_09E4:
                if (num8 <= num47)
                {
                    goto Label_0982;
                }
                stream4 = new FileStream(str5, 3, 1);
                reader3 = new BinaryReader(stream4);
                reader3.BaseStream.Seek(0L, 0);
                num8 = 0;
            Label_0A11:
                num25 = reader3.ReadByte();
                if (num25 != 0)
                {
                    goto Label_0A22;
                }
                flag3 = 1;
            Label_0A22:
                if (((num25 > 0x40) & ((num25 > 0x7b) == 0)) == null)
                {
                    goto Label_0A4D;
                }
                if (flag3 != null)
                {
                    goto Label_0A4D;
                }
                str7 = str7 + Conversions.ToString(Strings.Chr(num25));
            Label_0A4D:
                num8 += 1;
                if (num8 <= 0x3f)
                {
                    goto Label_0A11;
                }
                &this.vh.header_text = str7;
                &this.vh.nVertice_count = reader3.ReadUInt32();
                flag3 = 0;
                str7 = "";
                if (Strings.InStr(&this.vh.header_text, "xyznuviiiwwtb", 0) <= 0)
                {
                    goto Label_0AA4;
                }
                Globals.stride = 0x25;
                goto Label_0AAB;
            Label_0AA4:
                Globals.stride = 0x20;
            Label_0AAB:
                if (Globals.has_uv2 == null)
                {
                    goto Label_0AC5;
                }
                this.get_uv2(&this.vh.nVertice_count, str4);
            Label_0AC5:
                if (_add != null)
                {
                    goto Label_0AF1;
                }
                Globals.object_start = 1;
                num = &this.ih.nInd_groups;
                Globals.object_table = this.DataSet1.objtbl.Clone();
                goto Label_0B10;
            Label_0AF1:
                Globals.object_start = Globals.object_count + 1;
                num = (uint)(Globals.object_count + &this.ih.nInd_groups);
            Label_0B10:
                Globals._object = (Classes.obj[])Utils.CopyArray((Array)Globals._object, new Classes.obj[((int)num) + 1]);
                num48 = num;
                num8 = (uint)Globals.object_start;
                goto Label_0B53;
            Label_0B3F:
                Globals._object[(int)num8] = new Classes.obj();
                num8 += 1;
            Label_0B53:
                if (num8 <= num48)
                {
                    goto Label_0B3F;
                }
                Globals._group = (Globals._grps[])Utils.CopyArray((Array)Globals._group, new Globals._grps[((int)num) + 1]);
                e_Array = new Classes.vertice_[((int)&this.vh.nVertice_count) + 1];
                num8 = 0;
                num28 = 6;
                num49 = num;
                num33 = (uint)Globals.object_start;
                goto Label_10FB;
            Label_0BA6:
                if (flag2 == null)
                {
                    goto Label_0C45;
                }
                num34 = (uint)(&(groupArray[0]).nVertices_ - 1L);
                &(Globals._group[(int)num33]).startVertex_ = (uint)&(groupArray[0]).startVertex_;
                &(Globals._group[(int)num33]).startIndex_ = (uint)&(groupArray[0]).startIndex_;
                &(Globals._group[(int)num33]).nVertices_ = (uint)&(groupArray[0]).nVertices_;
                &(Globals._group[(int)num33]).nPrimitives_ = (uint)&(groupArray[0]).nPrimitives_;
                goto Label_0D0A;
            Label_0C45:
                num34 = (uint)(&(groupArray[(int)(((ulong)num33) - ((long)Globals.object_start))]).nVertices_ - 1L);
                &(Globals._group[(int)num33]).startVertex_ = (uint)&(groupArray[(int)(((ulong)num33) - ((long)Globals.object_start))]).startVertex_;
                &(Globals._group[(int)num33]).startIndex_ = (uint)&(groupArray[(int)(((ulong)num33) - ((long)Globals.object_start))]).startIndex_;
                &(Globals._group[(int)num33]).nVertices_ = (uint)&(groupArray[(int)(((ulong)num33) - ((long)Globals.object_start))]).nVertices_;
                &(Globals._group[(int)num33]).nPrimitives_ = (uint)&(groupArray[(int)(((ulong)num33) - ((long)Globals.object_start))]).nPrimitives_;
            Label_0D0A:
                &(Globals._group[(int)num33]).vertices = new Classes.vertice_[((int)(((ulong)num34) + 1L)) + 1];
                num50 = num34;
                num3 = 0;
                goto Label_10EC;
            Label_0D36:
                e_Array[(int)num8] = new Classes.vertice_();
                e_Array[(int)num8].x = reader3.ReadSingle() * -1f;
                e_Array[(int)num8].y = reader3.ReadSingle();
                e_Array[(int)num8].z = reader3.ReadSingle();
                e_Array[(int)num8].n = reader3.ReadUInt32();
                e_Array[(int)num8].u = reader3.ReadSingle();
                e_Array[(int)num8].v = reader3.ReadSingle();
                if (Strings.InStr(&this.vh.header_text, "xyznuviiiwwtb", 0) <= 0)
                {
                    goto Label_0E4F;
                }
                e_Array[(int)num8].index_1 = reader3.ReadByte();
                e_Array[(int)num8].index_2 = reader3.ReadByte();
                e_Array[(int)num8].index_3 = reader3.ReadByte();
                e_Array[(int)num8].weight_1 = reader3.ReadByte();
                e_Array[(int)num8].weight_2 = reader3.ReadByte();
                e_Array[(int)num8].t = reader3.ReadUInt32();
                e_Array[(int)num8].bn = reader3.ReadUInt32();
                goto Label_0E73;
            Label_0E4F:
                e_Array[(int)num8].t = reader3.ReadUInt32();
                e_Array[(int)num8].bn = reader3.ReadUInt32();
            Label_0E73:
                &(Globals._group[(int)num33]).vertices[(int)num3] = new Classes.vertice_();
                &(Globals._group[(int)num33]).vertices[(int)num3].x = e_Array[(int)num8].x;
                &(Globals._group[(int)num33]).vertices[(int)num3].y = e_Array[(int)num8].y;
                &(Globals._group[(int)num33]).vertices[(int)num3].z = e_Array[(int)num8].z;
                &(Globals._group[(int)num33]).vertices[(int)num3].n = e_Array[(int)num8].n;
                &(Globals._group[(int)num33]).vertices[(int)num3].u = e_Array[(int)num8].u;
                &(Globals._group[(int)num33]).vertices[(int)num3].v = e_Array[(int)num8].v;
                &(Globals._group[(int)num33]).vertices[(int)num3].index_1 = e_Array[(int)num8].index_1;
                &(Globals._group[(int)num33]).vertices[(int)num3].index_2 = e_Array[(int)num8].index_2;
                &(Globals._group[(int)num33]).vertices[(int)num3].index_3 = e_Array[(int)num8].index_3;
                &(Globals._group[(int)num33]).vertices[(int)num3].weight_1 = e_Array[(int)num8].weight_1;
                &(Globals._group[(int)num33]).vertices[(int)num3].weight_2 = e_Array[(int)num8].weight_2;
                &(Globals._group[(int)num33]).vertices[(int)num3].t = e_Array[(int)num8].t;
                &(Globals._group[(int)num33]).vertices[(int)num3].bn = e_Array[(int)num8].bn;
                if (Globals.has_uv2 == null)
                {
                    goto Label_10DD;
                }
                &(Globals._group[(int)num33]).vertices[(int)num3].u2 = &(Globals.uv2[(int)num8]).u;
                &(Globals._group[(int)num33]).vertices[(int)num3].v2 = &(Globals.uv2[(int)num8]).v;
            Label_10DD:
                num8 = (uint)(((ulong)num8) + 1L);
                num3 += 1;
            Label_10EC:
                if (num3 <= num50)
                {
                    goto Label_0D36;
                }
                num33 += 1;
            Label_10FB:
                if (num33 <= num49)
                {
                    goto Label_0BA6;
                }
                stream4.Close();
                stream4.Dispose();
                stream4 = null;
                num26 = 0;
                num29 = 0;
                num32 = 0;
                this.pbar.Visible = 1;
                num51 = (uint)(&this.ih.nInd_groups - 1);
                num8 = 0;
                goto Label_115B;
            Label_113F:
                num32 = (uint)(((ulong)num32) + &(groupArray[(int)num8]).nPrimitives_);
                num8 += 1;
            Label_115B:
                if (num8 <= num51)
                {
                    goto Label_113F;
                }
                this.pbar.Maximum = (int)Math.Round(((double)((double)Globals.master_cnt)) + (((double)((double)num32)) / 4.0));
                this.pbar.Minimum = (int)Globals.master_cnt;
                this.pbar.Value = this.pbar.Minimum;
                num31 = 0;
                num52 = num;
                num9 = (uint)Globals.object_start;
                goto Label_2038;
            Label_11C2:
                Globals.object_count += 1;
                vis_main.get_textures_and_names((int)num9, num12);
                this.build_textures((int)num9, 0);
                this.obj_count.Text = Conversions.ToString(num9);
                this.obj_sel.Maximum = new decimal(num9);
                num9 = (uint)(((ulong)num9) - 1L);
                Globals._object[(int)(((ulong)num9) + 1L)].name = "Primitive: " + &num9.ToString();
                num9 = (uint)(((ulong)num9) + 1L);
                Globals._object[(int)num9].ID = num9;
                num3 = (uint)&(groupArray[(int)(((ulong)num9) - ((long)Globals.object_start))]).nPrimitives_;
                &(Globals._group[(int)num9]).indicies = (Globals.uvect3[])Utils.CopyArray((Array) & (Globals._group[(int)num9]).indicies, new Globals.uvect3[((int)num3) + 1]);
                reader2.BaseStream.Seek((&(groupArray[(int)(((ulong)num9) - ((long)Globals.object_start))]).startIndex_ * ((ulong)Globals.ind_scale)) + 0x48L, 0);
                Globals._object[(int)num9].tris = (Classes.triangle[])Utils.CopyArray((Array)Globals._object[(int)num9].tris, new Classes.triangle[((int)num3) + 1]);
                Globals._object[(int)num9].vd_1 = (Classes.verts[])Utils.CopyArray((Array)Globals._object[(int)num9].vd_1, new Classes.verts[((int)num3) + 1]);
                Globals._object[(int)num9].vd_2 = (Classes.verts[])Utils.CopyArray((Array)Globals._object[(int)num9].vd_2, new Classes.verts[((int)num3) + 1]);
                Globals._object[(int)num9].vd_3 = (Classes.verts[])Utils.CopyArray((Array)Globals._object[(int)num9].vd_3, new Classes.verts[((int)num3) + 1]);
                Globals._object[(int)num9].count = num3;
                Globals._object[(int)num9].old_count = num3;
                num53 = num3;
                num8 = 1;
                goto Label_1EA1;
            Label_13D2:
                Globals.master_cnt = (uint)(((ulong)Globals.master_cnt) + 1L);
                this.pbar.Value = (int)Math.Round(((double)this.pbar.Minimum) + (((double)((double)num31)) / 4.0));
                num31 = (uint)(((ulong)num31) + 1L);
                Globals._object[(int)num9].tris[(int)num8] = new Classes.triangle();
                Globals._object[(int)num9].vd_1[(int)num8] = new Classes.verts();
                Globals._object[(int)num9].vd_2[(int)num8] = new Classes.verts();
                Globals._object[(int)num9].vd_3[(int)num8] = new Classes.verts();
                Globals._object[(int)num9].tris[(int)num8].id = num8;
                vect = new Globals.vect3();
                num35 = 0x20;
                if (((ulong)Globals.ind_scale) != 2L)
                {
                    goto Label_14BE;
                }
                num37 = reader2.ReadUInt16();
                num36 = reader2.ReadUInt16();
                num38 = reader2.ReadUInt16();
                goto Label_14D9;
            Label_14BE:
                num37 = reader2.ReadUInt32();
                num36 = reader2.ReadUInt32();
                num38 = reader2.ReadUInt32();
            Label_14D9:
                &(Globals._group[(int)num9]).indicies[(int)num8] = new Globals.uvect3();
                &(&(Globals._group[(int)num9]).indicies[(int)num8]).v1 = num36;
                &(&(Globals._group[(int)num9]).indicies[(int)num8]).v2 = num37;
                &(&(Globals._group[(int)num9]).indicies[(int)num8]).v3 = num38;
                Globals._object[(int)num9].tris[(int)num8].c1x = e_Array[(int)num36].x;
                Globals._object[(int)num9].tris[(int)num8].c1y = e_Array[(int)num36].y;
                Globals._object[(int)num9].tris[(int)num8].c1z = e_Array[(int)num36].z;
                Globals._object[(int)num9].vd_1[(int)num8].x = e_Array[(int)num36].x;
                Globals._object[(int)num9].vd_1[(int)num8].y = e_Array[(int)num36].y;
                Globals._object[(int)num9].vd_1[(int)num8].z = e_Array[(int)num36].z;
                vect = this.unpackNormal(e_Array[(int)num36].n);
                Globals._object[(int)num9].tris[(int)num8].n1x = &vect.x;
                Globals._object[(int)num9].tris[(int)num8].n1y = &vect.y;
                Globals._object[(int)num9].tris[(int)num8].n1z = &vect.z;
                Globals._object[(int)num9].tris[(int)num8].u1 = e_Array[(int)num36].u;
                Globals._object[(int)num9].tris[(int)num8].v1 = e_Array[(int)num36].v;
                Globals._object[(int)num9].tris[(int)num8].bn = e_Array[(int)num36].bn;
                Globals._object[(int)num9].tris[(int)num8].tan = e_Array[(int)num36].t;
                if (Globals.has_uv2 == null)
                {
                    goto Label_17FD;
                }
                Globals._object[(int)num9].tris[(int)num8].u2_1 = &(Globals.uv2[(int)num36]).u;
                Globals._object[(int)num9].tris[(int)num8].v2_1 = &(Globals.uv2[(int)num36]).v;
                Globals._object[(int)num9].vd_1[(int)num8].u = &(Globals.uv2[(int)num36]).u;
                Globals._object[(int)num9].vd_1[(int)num8].v = &(Globals.uv2[(int)num36]).v;
                Globals._object[(int)num9].vd_1[(int)num8].id = num36;
                Globals._object[(int)num9].tris[(int)num8].uv2_id_1 = num36;
            Label_17FD:
                Globals._object[(int)num9].tris[(int)num8].c2x = e_Array[(int)num37].x;
                Globals._object[(int)num9].tris[(int)num8].c2y = e_Array[(int)num37].y;
                Globals._object[(int)num9].tris[(int)num8].c2z = e_Array[(int)num37].z;
                Globals._object[(int)num9].vd_2[(int)num8].x = e_Array[(int)num37].x;
                Globals._object[(int)num9].vd_2[(int)num8].y = e_Array[(int)num37].y;
                Globals._object[(int)num9].vd_2[(int)num8].z = e_Array[(int)num37].z;
                vect = this.unpackNormal(e_Array[(int)num37].n);
                Globals._object[(int)num9].tris[(int)num8].n2x = &vect.x;
                Globals._object[(int)num9].tris[(int)num8].n2y = &vect.y;
                Globals._object[(int)num9].tris[(int)num8].n2z = &vect.z;
                Globals._object[(int)num9].tris[(int)num8].u2 = e_Array[(int)num37].u;
                Globals._object[(int)num9].tris[(int)num8].v2 = e_Array[(int)num37].v;
                Globals._object[(int)num9].tris[(int)num8].bn = e_Array[(int)num37].bn;
                Globals._object[(int)num9].tris[(int)num8].tan = e_Array[(int)num37].t;
                if (Globals.has_uv2 == null)
                {
                    goto Label_1A9E;
                }
                Globals._object[(int)num9].tris[(int)num8].u2_2 = &(Globals.uv2[(int)num37]).u;
                Globals._object[(int)num9].tris[(int)num8].v2_2 = &(Globals.uv2[(int)num37]).v;
                Globals._object[(int)num9].vd_2[(int)num8].u = &(Globals.uv2[(int)num37]).u;
                Globals._object[(int)num9].vd_2[(int)num8].v = &(Globals.uv2[(int)num37]).v;
                Globals._object[(int)num9].vd_2[(int)num8].id = num37;
                Globals._object[(int)num9].tris[(int)num8].uv2_id_2 = num37;
            Label_1A9E:
                Globals._object[(int)num9].tris[(int)num8].c3x = e_Array[(int)num38].x;
                Globals._object[(int)num9].tris[(int)num8].c3y = e_Array[(int)num38].y;
                Globals._object[(int)num9].tris[(int)num8].c3z = e_Array[(int)num38].z;
                Globals._object[(int)num9].vd_3[(int)num8].x = e_Array[(int)num38].x;
                Globals._object[(int)num9].vd_3[(int)num8].y = e_Array[(int)num38].y;
                Globals._object[(int)num9].vd_3[(int)num8].z = e_Array[(int)num38].z;
                vect = this.unpackNormal(e_Array[(int)num38].n);
                Globals._object[(int)num9].tris[(int)num8].n3x = &vect.x;
                Globals._object[(int)num9].tris[(int)num8].n3y = &vect.y;
                Globals._object[(int)num9].tris[(int)num8].n3z = &vect.z;
                Globals._object[(int)num9].tris[(int)num8].u3 = e_Array[(int)num38].u;
                Globals._object[(int)num9].tris[(int)num8].v3 = e_Array[(int)num38].v;
                Globals._object[(int)num9].tris[(int)num8].bn = e_Array[(int)num38].bn;
                Globals._object[(int)num9].tris[(int)num8].tan = e_Array[(int)num38].t;
                if (Globals.has_uv2 == null)
                {
                    goto Label_1D3F;
                }
                Globals._object[(int)num9].tris[(int)num8].u2_3 = &(Globals.uv2[(int)num38]).u;
                Globals._object[(int)num9].tris[(int)num8].v2_3 = &(Globals.uv2[(int)num38]).v;
                Globals._object[(int)num9].vd_3[(int)num8].u = &(Globals.uv2[(int)num38]).u;
                Globals._object[(int)num9].vd_3[(int)num8].v = &(Globals.uv2[(int)num38]).v;
                Globals._object[(int)num9].vd_3[(int)num8].id = num38;
                Globals._object[(int)num9].tris[(int)num8].uv2_id_3 = num38;
            Label_1D3F:
                if (Globals._object[(int)num9].tris[(int)num8].c1x >= Globals.x_min)
                {
                    goto Label_1D79;
                }
                Globals.x_min = Globals._object[(int)num9].tris[(int)num8].c1x;
            Label_1D79:
                if (Globals._object[(int)num9].tris[(int)num8].c1x <= Globals.x_max)
                {
                    goto Label_1DB3;
                }
                Globals.x_max = Globals._object[(int)num9].tris[(int)num8].c1x;
            Label_1DB3:
                if (Globals._object[(int)num9].tris[(int)num8].c1y >= Globals.y_min)
                {
                    goto Label_1DED;
                }
                Globals.y_min = Globals._object[(int)num9].tris[(int)num8].c1y;
            Label_1DED:
                if (Globals._object[(int)num9].tris[(int)num8].c1y <= Globals.y_max)
                {
                    goto Label_1E27;
                }
                Globals.y_max = Globals._object[(int)num9].tris[(int)num8].c1y;
            Label_1E27:
                if (Globals._object[(int)num9].tris[(int)num8].c1z >= Globals.z_min)
                {
                    goto Label_1E61;
                }
                Globals.z_min = Globals._object[(int)num9].tris[(int)num8].c1z;
            Label_1E61:
                if (Globals._object[(int)num9].tris[(int)num8].c1z <= Globals.z_max)
                {
                    goto Label_1E9B;
                }
                Globals.z_max = Globals._object[(int)num9].tris[(int)num8].c1z;
            Label_1E9B:
                num8 += 1;
            Label_1EA1:
                if (num8 <= num53)
                {
                    goto Label_13D2;
                }
                row = Globals.object_table.NewRow();
                row[0] = (uint)num9;
                row[1] = Globals._object[(int)num9].name;
                row[2] = (uint)num3;
                Globals.object_table.Rows.Add(row);
                this.Object_tableBindingsource.DataSource = Globals.object_table;
                Globals._object[(int)num9].color.red = 0.6f;
                Globals._object[(int)num9].color.blue = 0.6f;
                Globals._object[(int)num9].color.green = 0.6f;
                num12 += 1;
                if (_add != null)
                {
                    goto Label_1FCE;
                }
                Globals.eye_x = (Globals.x_min + Globals.x_max) / 2f;
                Globals.eye_y = (Globals.y_min + Globals.y_max) / 2f;
                Globals.eye_z = (Globals.z_min + Globals.z_max) / 2f;
                Globals.look_radius = (float)(((double)(Globals.y_max + Globals.x_max)) * -1.5);
                Globals.Look_X_angle = 2.356194f;
                Globals.Look_Y_angle = -0.5235988f;
            Label_1FCE:
                if (this.fix_normals.Checked == null)
                {
                    goto Label_1FE4;
                }
                this._normalize((int)num9);
            Label_1FE4:
                Application.DoEvents();
                this.make_lists();
                Globals.stop_opengl = 0;
                this.draw_scene();
                Application.DoEvents();
                Application.DoEvents();
                Application.DoEvents();
                Application.DoEvents();
                Globals.stop_opengl = 1;
                Globals._object[(int)num9].find_center();
                Globals._object[(int)num9].modified = 0;
                num9 += 1;
            Label_2038:
                if (num9 <= num52)
                {
                    goto Label_11C2;
                }
                Application.DoEvents();
                reader2.Close();
                reader3.Close();
                reader2 = null;
                reader3 = null;
                stream = null;
                stream4 = null;
                this.poly_count.Text = Conversions.ToString(Globals.master_cnt);
                _add = 1;
                if (num15 <= 0)
                {
                    goto Label_2116;
                }
                if ((Globals.has_uv2 | Globals.has_color) == null)
                {
                    goto Label_20A0;
                }
                num8 = (uint)((((int)Globals.section_names.Length) - 1) - (num15 * 3));
                goto Label_20B1;
            Label_20A0:
                num8 = (uint)((((int)Globals.section_names.Length) - 1) - (num15 * 2));
            Label_20B1:
                str5 = @"C:\wot_temp\" + Globals.section_names[(int)num8] + ".sec";
                if ((Globals.has_uv2 | Globals.has_color) == null)
                {
                    goto Label_20F8;
                }
                str3 = @"C:\wot_temp\" + Globals.section_names[(int)(((ulong)num8) + 2L)] + ".sec";
                goto Label_2116;
            Label_20F8:
                str3 = @"C:\wot_temp\" + Globals.section_names[(int)(((ulong)num8) + 1L)] + ".sec";
            Label_2116:
                if (num15 > 0)
                {
                    goto Label_07D7;
                }
                goto Label_2141;
            }
            catch (Exception exception2)
            {
            Label_2120:
                ProjectData.SetProjectError(exception2);
                exception = exception2;
                Interaction.MsgBox("This model has a unrecognized format. I can't open it.", 0x30, "File format error");
                ProjectData.ClearProjectError();
                goto Label_2141;
            }
        Label_2141:
            _add = flag;
            this.obj_sel.Enabled = 1;
            this.obj_sel.Minimum = decimal.One;
            this.n_red.Enabled = 1;
            this.n_green.Enabled = 1;
            this.n_blue.Enabled = 1;
            this.get_total_verts();
            this.draw_scene();
            this.pb1.Refresh();
            this.pbar.Visible = 0;
            array2 = this.TextBox2.Text.Split(new char[] { 0x5c });
            num10 = array2.Length - 1;
            str8 = "";
            num54 = num10 - 1;
            num39 = 0;
            goto Label_2222;
        Label_21E7: ;
            str8 = Conversions.ToString(Operators.AddObject(str8, Operators.ConcatenateObject(NewLateBinding.LateIndexGet(array2, new object[] { (int)num39 }, null), @"\")));
            num39 += 1;
        Label_2222:
            if (num39 <= num54)
            {
                goto Label_21E7;
            }
            str9 = Conversions.ToString(NewLateBinding.LateIndexGet(array2, new object[] { (int)num10 }, null));
            array2 = str9.Split(new char[] { 0x2e });
            str8 = Conversions.ToString(Operators.AddObject(str8, Operators.ConcatenateObject(NewLateBinding.LateIndexGet(array2, new object[] { (int)0 }, null), "_out.txt")));
            this.Text = "File: " + str9;
            this.set_eyes();
        Label_22B1:
            return;
        }
#endif
    }
}


