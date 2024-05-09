using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf3dLib;


namespace WPFPlayground
{
    /// <summary>
    /// Interaction logic for Garden3D.xaml
    /// </summary>
    public partial class Garden3D : Window
    {
        //readonly Random _rand = new();
        //int _lastTick = Environment.TickCount;

        // The main model group.
        readonly Model3DGroup _group;

        // The camera.
        readonly PerspectiveCamera _camera;

        // The camera controller.
        readonly SphericalCameraController _cameraController;

        // Where the resources be.
        string _resDir = "";

        public Garden3D()
        {
            InitializeComponent();

            Left = 200;
            Top = 10;

            _group = new Model3DGroup();

            // Define the camera.
            _camera = new() { FieldOfView = 60 };
            _cameraController = new(_camera, mainViewport, this, mainGrid, mainGrid);
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _resDir = System.IO.Path.Combine(Utils.GetSourcePath(), "Resources");

            // Define WPF objects.
            ModelVisual3D visual3d = new();
            visual3d.Content = _group;
            mainViewport.Children.Add(visual3d);

            // Define the lights.
            Color darker = Color.FromArgb(255, 96, 96, 96);
            Color dark = Color.FromArgb(255, 128, 128, 128);
            _group.Children.Add(new AmbientLight(darker));
            _group.Children.Add(new DirectionalLight(dark, new Vector3D( 0, -1,  0)));
            _group.Children.Add(new DirectionalLight(dark, new Vector3D( 1, -3, -2)));
            _group.Children.Add(new DirectionalLight(dark, new Vector3D(-1,  3,  2)));

            // Define the model.
            DefineModelGarden();
        }

        /// Define the model.
        void DefineModelGarden()
        {
            Width = 700;
            Height = 700;

            // Rock sections.
            MeshGeometry3D rockMesh = new();
            AddRectangle(rockMesh,
                new Point3D(-3, 0, -1),
                new Point3D(-3, 0, +1),
                new Point3D(-1, 0, +1),
                new Point3D(-1, 0, -1));
            AddRectangle(rockMesh,
                new Point3D(+1, 0, -1),
                new Point3D(+1, 0, +1),
                new Point3D(+3, 0, +1),
                new Point3D(+3, 0, -1));
            AddRectangle(rockMesh,
                new Point3D(-1, 0, +1),
                new Point3D(-1, 0, +3),
                new Point3D(+1, 0, +3),
                new Point3D(+1, 0, +1));

            ImageBrush rockBrush = new();
            rockBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"rocks.jpg"), UriKind.Relative));
            var rockMaterial = new DiffuseMaterial(rockBrush);
            GeometryModel3D rockModel = new(rockMesh, rockMaterial);

            _group.Children.Add(rockModel);

            // Grass sections.
            MeshGeometry3D grassMesh = new();
            AddRectangle(grassMesh,
                new Point3D(-3, 0, -3),
                new Point3D(-3, 0, -1),
                new Point3D(-1, 0, -1),
                new Point3D(-1, 0, -3));
            AddRectangle(grassMesh,
                new Point3D(-3, 0, +1),
                new Point3D(-3, 0, +3),
                new Point3D(-1, 0, +3),
                new Point3D(-1, 0, +1));
            AddRectangle(grassMesh,
                new Point3D(+1, 0, -3),
                new Point3D(+1, 0, -1),
                new Point3D(+3, 0, -1),
                new Point3D(+3, 0, -3));
            AddRectangle(grassMesh,
                new Point3D(+1, 0, +1),
                new Point3D(+1, 0, +3),
                new Point3D(+3, 0, +3),
                new Point3D(+3, 0, +1));

            ImageBrush grassBrush = new();
            grassBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"grass.jpg"), UriKind.Relative));
            Material grassMaterial = new DiffuseMaterial(grassBrush);
            GeometryModel3D grassModel = new(grassMesh, grassMaterial);
            _group.Children.Add(grassModel);

            // Water.
            MeshGeometry3D waterMesh = new();
            AddRectangle(waterMesh,
                new Point3D(-1, 0, -1),
                new Point3D(-1, 0, +1),
                new Point3D(+1, 0, +1),
                new Point3D(+1, 0, -1));
            ImageBrush waterBrush = new();
            waterBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"water.jpg"), UriKind.Relative));
            Material waterMaterial = new DiffuseMaterial(waterBrush);
            GeometryModel3D waterModel = new(waterMesh, waterMaterial);
            _group.Children.Add(waterModel);

            // Cube brick face.
            MeshGeometry3D brickMesh = new();
            AddRectangle(brickMesh,
                new Point3D(-1, 2, -1),
                new Point3D(-1, 0, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 2, -1));
            ImageBrush brickBrush = new();
            brickBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"bricks.jpg"), UriKind.Relative));
            Material brickMaterial = new DiffuseMaterial(brickBrush);
            GeometryModel3D brickModel = new(brickMesh, brickMaterial);
            _group.Children.Add(brickModel);

            // Cube metal face.
            MeshGeometry3D metalMesh = new();
            AddRectangle(metalMesh,
                new Point3D(+1, 2, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 0, -3),
                new Point3D(+1, 2, -3));
            ImageBrush metalBrush = new();
            metalBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"metal.jpg"), UriKind.Relative));
            Material metalMaterial = new DiffuseMaterial(metalBrush);
            GeometryModel3D metalModel = new(metalMesh, metalMaterial);
            _group.Children.Add(metalModel);

            // Cube wood face.
            MeshGeometry3D woodMesh = new();
            AddRectangle(woodMesh,
                new Point3D(-1, 2, -3),
                new Point3D(-1, 2, -1),
                new Point3D(+1, 2, -1),
                new Point3D(+1, 2, -3));
            ImageBrush woodBrush = new();
            woodBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"wood.jpg"), UriKind.Relative));
            Material woodMaterial = new DiffuseMaterial(woodBrush);
            GeometryModel3D woodModel = new(woodMesh, woodMaterial);
            _group.Children.Add(woodModel);

            // Cube fire face.
            MeshGeometry3D fireMesh = new();
            AddRectangle(fireMesh,
                new Point3D(-1, 2, -3),
                new Point3D(-1, 0, -3),
                new Point3D(-1, 0, -1),
                new Point3D(-1, 2, -1));
            ImageBrush fireBrush = new();
            fireBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"fire.jpg"), UriKind.Relative));
            Material fireMaterial = new DiffuseMaterial(fireBrush);
            GeometryModel3D fireModel = new(fireMesh, fireMaterial);
            _group.Children.Add(fireModel);

            // Cube cloth face.
            MeshGeometry3D clothMesh = new();
            AddRectangle(clothMesh,
                new Point3D(+1, 2, -3),
                new Point3D(+1, 0, -3),
                new Point3D(-1, 0, -3),
                new Point3D(-1, 2, -3));
            ImageBrush clothBrush = new();
            clothBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"cloth.jpg"), UriKind.Relative));
            Material clothMaterial = new DiffuseMaterial(clothBrush);
            GeometryModel3D clothModel = new(clothMesh, clothMaterial);
            _group.Children.Add(clothModel);

            // Skybox meshes.
            MeshGeometry3D sky1Mesh = new();
            AddRectangle(sky1Mesh,
                new Point3D(-6, +7, +6),
                new Point3D(-6, -5, +6),
                new Point3D(-6, -5, -6),
                new Point3D(-6, +7, -6));
            ImageBrush sky1Brush = new();
            sky1Brush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"clouds.jpg"), UriKind.Relative));
            MaterialGroup sky1Group = new();
            sky1Group.Children.Add(new DiffuseMaterial(sky1Brush));
            sky1Group.Children.Add(new EmissiveMaterial(new SolidColorBrush(
                Color.FromArgb(255, 128, 128, 128))));
            GeometryModel3D sky1Model = new(sky1Mesh, sky1Group);
            _group.Children.Add(sky1Model);

            MeshGeometry3D sky2Mesh = new();
            AddRectangle(sky2Mesh,
                new Point3D(-6, +7, -6),
                new Point3D(-6, -5, -6),
                new Point3D(+6, -5, -6),
                new Point3D(+6, +7, -6));
            ImageBrush sky2Brush = new();
            sky2Brush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"clouds.jpg"), UriKind.Relative));
            MaterialGroup sky2Group = new();
            sky2Group.Children.Add(new DiffuseMaterial(sky2Brush));
            sky2Group.Children.Add(new EmissiveMaterial(new SolidColorBrush(
                Color.FromArgb(255, 64, 64, 64))));
            GeometryModel3D sky2Model = new(sky2Mesh, sky2Group);
            _group.Children.Add(sky2Model);

            MeshGeometry3D sky3Mesh = new();
            AddRectangle(sky3Mesh,
                new Point3D(-6, -5, +6),
                new Point3D(+6, -5, +6),
                new Point3D(+6, -5, -6),
                new Point3D(-6, -5, -6));
            ImageBrush sky3Brush = new();
            sky3Brush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"clouds.jpg"), UriKind.Relative));
            Material sky3Material = new DiffuseMaterial(sky3Brush);
            GeometryModel3D sky3Model = new(sky3Mesh, sky3Material);
            _group.Children.Add(sky3Model);
        }

        /// Add a rectangle with texture coordinates to the mesh.
        void AddRectangle(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.Positions.Add(p4);

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 2);
            mesh.TriangleIndices.Add(index + 3);
        }
    }
}
