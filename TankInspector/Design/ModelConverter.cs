using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Design
{
    public class ModelConverter : IMultiValueConverter
    {
	    private static Dictionary<string, ImageSource> _materialImages = new Dictionary<string, ImageSource>();
	    private static Dictionary<string, Task<string>> _loadingImages = new Dictionary<string, Task<string>>();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#if WITHOUT_D3D
            var rawModel = values[0];
            var rawModel3d = values[1];
            if (rawModel3d == null)
            {
                rawModel3d = new Model3DGroup();
            }

            if (rawModel is Model && rawModel3d is Model3D)
            {
                var dispatcher = Dispatcher.CurrentDispatcher;
                var context = TaskScheduler.FromCurrentSynchronizationContext();
                var contextTask = Task.Factory.StartNew(() => { }, CancellationToken.None, TaskCreationOptions.None, context);
                var model = (Model)rawModel;
                var lastModel3D=(Model3DGroup)rawModel3d;
                var model3D = new Model3DGroup();

                model.LoadingTask.ContinueWith((_) =>
                        {

                            var tracks = new List<ModelRenderSet>();
                            var all = new List<ModelRenderSet>();


                            foreach (var renderSet in model.Visual.RenderSets)
                            {
                                if (renderSet.Geometry.PrimitiveName.Contains("track"))
                                {
                                    tracks.Add(renderSet);
                                }
                                else
                                {
                                    all.Add(renderSet);
                                }
                            }

                            all.AddRange(tracks);

                            foreach (var renderSet in all)
                            {
                                var geos = GetModel(renderSet, model);

                                var nextTask = contextTask.ContinueWith(
                                    (prevTask) =>
                                    {
                                        foreach (var geo in geos)
                                        {
                                            model3D.Children.Add(geo);
                                        }
                                    }, context);
                                contextTask = nextTask;
                            }
                        }).ContinueWith((_) =>
                        {
                            var nextTask = contextTask.ContinueWith(
                                (prevTask) =>
                                {
                                    lastModel3D.Children.Clear();
                                    lastModel3D.Children = model3D.Children;
                                }, context);
                            contextTask = nextTask;
                        }); 

                return rawModel3d;
            }
            else if (rawModel3d is Model3D)
            {
                return rawModel3d;
            }
#endif
            return null;
        }
        

#if WITHOUT_D3D
        private static List<GeometryModel3D> GetModel(ModelRenderSet renderSet,Model model)        
        {
            var geoModels = new List<GeometryModel3D>();
            try
            { 
                foreach (var groupKV in renderSet.Geometry.ModelPrimitiveGroups)
                {
                    var group = groupKV.Value;

                    var material = GetMaterial(group, model);
                    if (material.Children.Count == 0)
                    {
                        continue;
                    }

                    var mesh = GetMesh(renderSet, group);
                    var geoModel = new GeometryModel3D();
                    ModelViewModel.SetModel(geoModel, model);
                    geoModel.Geometry = mesh;
                    geoModel.Material = material;
                    if (model.Type == Model.ModelType.Collision)
                    {
                        geoModel.BackMaterial = material;
                    }
                    geoModel.Freeze();
                    geoModels.Add(geoModel);
                }
                
            }
            catch (Exception)
            {
                Trace.WriteLine(string.Format("{0} can't render this group.", renderSet.Geometry.ModelPrimitiveGroups[0].Material.Fx));
                
            }
            return geoModels;
        }

        static private double Gamma = 0.80;
        static private double IntensityMax = 255;

        /** Taken from Earl F. Glynn's web page:
        * <a href="http://www.efg2.com/Lab/ScienceAndEngineering/Spectra.htm">Spectra Lab Report</a>
        * */
        public static byte[] waveLengthToRGB(double Wavelength)
        {
            double factor;
            double Red, Green, Blue;

            if ((Wavelength >= 380) && (Wavelength < 440))
            {
                Red = -(Wavelength - 440) / (440 - 380);
                Green = 0.0;
                Blue = 1.0;
            }
            else if ((Wavelength >= 440) && (Wavelength < 490))
            {
                Red = 0.0;
                Green = (Wavelength - 440) / (490 - 440);
                Blue = 1.0;
            }
            else if ((Wavelength >= 490) && (Wavelength < 510))
            {
                Red = 0.0;
                Green = 1.0;
                Blue = -(Wavelength - 510) / (510 - 490);
            }
            else if ((Wavelength >= 510) && (Wavelength < 580))
            {
                Red = (Wavelength - 510) / (580 - 510);
                Green = 1.0;
                Blue = 0.0;
            }
            else if ((Wavelength >= 580) && (Wavelength < 645))
            {
                Red = 1.0;
                Green = -(Wavelength - 645) / (645 - 580);
                Blue = 0.0;
            }
            else if ((Wavelength >= 645) && (Wavelength < 781))
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 0.0;
            }
            else
            {
                Red = 0.0;
                Green = 0.0;
                Blue = 0.0;
            };

            // Let the intensity fall off near the vision limits

            if ((Wavelength >= 380) && (Wavelength < 420))
            {
                factor = 0.3 + 0.7 * (Wavelength - 380) / (420 - 380);
            }
            else if ((Wavelength >= 420) && (Wavelength < 701))
            {
                factor = 1.0;
            }
            else if ((Wavelength >= 701) && (Wavelength < 781))
            {
                factor = 0.3 + 0.7 * (780 - Wavelength) / (780 - 700);
            }
            else
            {
                factor = 0.0;
            };


            byte[] rgb = new byte[3];

            // Don't want 0^x = 1 for x <> 0
            rgb[0] = Red == 0.0 ? (byte)0 : (byte)Math.Round(IntensityMax * Math.Pow(Red * factor, Gamma));
            rgb[1] = Green == 0.0 ? (byte)0 : (byte)Math.Round(IntensityMax * Math.Pow(Green * factor, Gamma));
            rgb[2] = Blue == 0.0 ? (byte)0 : (byte)Math.Round(IntensityMax * Math.Pow(Blue * factor, Gamma));

            return rgb;
        }

        private int Adjust(double Color, double Factor, int IntensityMax, double Gamma)
        {
            if (Color == 0.0)
            {
                return 0;
            }
            else
            {
                return (int)Math.Round(IntensityMax * Math.Pow(Color * Factor, Gamma));
            }
        }

        private static MaterialGroup GetMaterial(ModelPrimitiveGroup group,Model model)
        {
            MaterialGroup material = new MaterialGroup();


            for (int i = group.Material.Propertys.Count - 1; i >= 0; --i)
            {
                var property = group.Material.Propertys[i];
                {
                    if (property.Name == "diffuseMap")
                    {
                        var diffusePath = new PackageFileInfo(property.Texture);

                        if (model.Type == Model.ModelType.Collision)
                        {
                            var armor = model.ArmorObject.Armor;
                            var armorValue = (byte)armor.GetArmorValue(group.Material.Identifier);

                            var rgb = waveLengthToRGB(armorValue + 410);

                            var diffuse = new DiffuseMaterial();
  
                            var diffuseBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,0,0,0));
                            diffuse.Brush = diffuseBrush;
                            diffuse.Color = Colors.Black;
                            diffuse.AmbientColor = Colors.Black;
                            material.Children.Add(diffuse);

                            var emissive = new EmissiveMaterial();

                            var emissiveBrush = new ImageBrush();
                            SetImageSource(emissiveBrush, diffusePath);
                            emissiveBrush.TileMode = TileMode.Tile;
                            emissiveBrush.ViewportUnits = BrushMappingMode.Absolute;
                            emissive.Brush = emissiveBrush;
                            material.Children.Add(emissive);
                        }
                        else
                        {
                            var diffuse = new DiffuseMaterial();
                            ImageBrush diffuseBrush = new ImageBrush();
                            SetImageSource(diffuseBrush, diffusePath);
                            diffuseBrush.TileMode = TileMode.Tile;
                            diffuseBrush.ViewportUnits = BrushMappingMode.Absolute;
                            diffuse.Brush = diffuseBrush;
                            material.Children.Add(diffuse);
                        }              
                    }
                    else if (property.Name == "specularMap")
                    {
                        var specularPath = new PackageFileInfo(property.Texture);
                        var specular = new SpecularMaterial();

                        ImageBrush specularBrush = new ImageBrush();
                        SetImageSource(specularBrush, specularPath);
                        specularBrush.TileMode = TileMode.Tile;
                        specularBrush.ViewportUnits = BrushMappingMode.Absolute;
                        specular.Brush = specularBrush;
                        specular.SpecularPower = 20;
                        material.Children.Add(specular);
                    }
                }
            }
            return material;
        }


        

        private static void SetImageSource(ImageBrush brush, PackageFileInfo path)
        {
            ImageSource ddsSource;

            if (!_materialImages.TryGetValue(path.ToString(), out ddsSource))
            {

                string packagePath;
                if (!PackageStream.IsFileExisted(path.PackagePath, path.FileInPackagePath))
                    packagePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(path.PackagePath)), "shared_content.pkg");
                else
                    packagePath = path.PackagePath;

                using (var Stream = new PackageStream(packagePath, path.FileInPackagePath))
                {
                    var dds = new DDSImage(Stream);
                    ddsSource = dds.BitmapSource(0);
                }
            }

            brush.ImageSource = ddsSource;
        }

        private static MeshGeometry3D GetMesh(ModelRenderSet renderSet, ModelPrimitiveGroup group)
        {
            var mesh = new MeshGeometry3D();

            mesh.Positions = renderSet.Geometry.Positions;
            mesh.TextureCoordinates = renderSet.Geometry.TextureCoordinates;
            mesh.Normals = renderSet.Geometry.Normals;

            if (!group.Sectioned)
            {
                mesh.TriangleIndices = renderSet.Geometry.TriangleIndices;
            }
            else //一般用来显示装甲
            {
                var primitive = renderSet.Geometry.TriangleIndices
                    .Where((v, i) => i >= group.StartIndex && i < group.EndIndex * 3);

                mesh.TriangleIndices = new Int32Collection(primitive);
            }

            return mesh;
        }

#endif

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
