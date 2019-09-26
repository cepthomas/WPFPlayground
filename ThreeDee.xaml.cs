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
    /// Interaction logic for ThreeDee.xaml
    /// </summary>
    public partial class ThreeDee : Window
    {
        enum ThreeDeeType { Garden, Robot }

        readonly Random _rand = new Random();
        int _lastTick = Environment.TickCount;

        // The main model group.
        Model3DGroup _group = null;

        // The robot's Model3DGroups.
        Model3DGroup _groupRobot, _groupHead, _groupNeck, _groupShoulder, _groupBack,
            _groupLeftUpperArm, _groupRightUpperArm, _groupLeftLowerArm, _groupRightLowerArm,
            _groupLeftUpperLeg, _groupRightUpperLeg, _groupLeftLowerLeg, _groupRightLowerLeg;

        // The camera.
        PerspectiveCamera _camera = null;

        // The camera controller.
        SphericalCameraController _cameraController = null;

        // Where the resources be.
        string _resDir = "";

        public ThreeDee()
        {
            InitializeComponent();

            Left = 200;
            Top = 10;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThreeDeeType which = ThreeDeeType.Robot;

            _resDir = System.IO.Path.Combine(Utils.GetSourcePath(), "Resources");

            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            _group = new Model3DGroup();
            visual3d.Content = _group;
            mainViewport.Children.Add(visual3d);

            // Define the camera.
            _camera = new PerspectiveCamera { FieldOfView = 60 };
            _cameraController = new SphericalCameraController(_camera, mainViewport, this, mainGrid, mainGrid);

            // Define the lights.
            Color darker = Color.FromArgb(255, 96, 96, 96);
            Color dark = Color.FromArgb(255, 128, 128, 128);
            _group.Children.Add(new AmbientLight(darker));
            _group.Children.Add(new DirectionalLight(dark, new Vector3D( 0, -1,  0)));
            _group.Children.Add(new DirectionalLight(dark, new Vector3D( 1, -3, -2)));
            _group.Children.Add(new DirectionalLight(dark, new Vector3D(-1,  3,  2)));

            ///// Define the model.
            if (which == ThreeDeeType.Robot)
            {
                // Move back a bit from the origin.
                Point3D coords = _cameraController.SphericalCoordinates;
                coords.X = 20;
                //coords.Y = 20;
                _cameraController.SphericalCoordinates = coords;
                DefineModelRobot();

                // Some animation.  red = x   green = y   blue = z

                // Mesh defines the surface.
                MeshGeometry3D animMesh = new MeshGeometry3D();
                Point3D pt = D3.Origin;

                //////// box
                int size = 2;
                animMesh.AddBox(pt, new Vector3D(size, 0, 0), new Vector3D(0, size, 0), new Vector3D(0, 0, size));

                //////// sphere
                //double radius = 2;
                //animMesh.AddSphere(pt, radius, 30, 10, true);
                //pt.Z += 2.5;
                //pt.X += 1;
                //animMesh.AddSphere(pt, radius, 30, 10, true);

                //////////// Common
                var dur = new Duration(TimeSpan.FromMilliseconds(5000));
                // Model is the thing that is manipulated.
                GeometryModel3D animModel = animMesh.MakeModel(Brushes.Violet);
                _group.Children.Add(animModel);
                var transGroup = new Transform3DGroup();
                animModel.Transform = transGroup;

                //////////// stretch
                ScaleTransform3D myScaleTransform3D = new ScaleTransform3D();
                myScaleTransform3D.ScaleX = 2;
                myScaleTransform3D.ScaleY = 0.5;
                myScaleTransform3D.ScaleZ = 1;
                // Add the scale transform to the Transform3DGroup.
                //myTransform3DGroup.Children.Add(myScaleTransform3D);
                transGroup.Children.Add(myScaleTransform3D);

                //////////// Move
                var anim = new DoubleAnimation(0.0, 3, dur)
                {
                    BeginTime = TimeSpan.FromSeconds(0),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                var offsetTransform = new TranslateTransform3D();
                //offsetTransform.BeginAnimation(TranslateTransform3D.OffsetXProperty, anim);
                //offsetTransform.BeginAnimation(TranslateTransform3D.OffsetYProperty, anim);
                offsetTransform.BeginAnimation(TranslateTransform3D.OffsetZProperty, anim);
                transGroup.Children.Add(offsetTransform);

                ///////// rotation
                var startAxis = new Vector3D(1, 0, 0);// (0, 1, 0);
                var rot = new AxisAngleRotation3D(startAxis, 180);
                var myRotateTransform = new RotateTransform3D(rot);
                // end, duration
                var rotateTo = new Vector3D(-1, -1, -1);
                var myVectorAnimation = new Vector3DAnimation(rotateTo, dur)
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };
                myRotateTransform.Rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, myVectorAnimation);
                transGroup.Children.Add(myRotateTransform);
            }
            else if (which == ThreeDeeType.Garden)
            {
                DefineModelGarden();
            }
        }

        #region Robot
        /// Define the model.
        void DefineModelRobot()
        {
            Width = 450;
            Height = 700;

            // Axes.
            MeshExtensions.AddXAxis(_group, 15, 0.1); // red = x
            MeshExtensions.AddYAxis(_group, 12, 0.1); // green = y
            MeshExtensions.AddZAxis(_group, 15, 0.1); // blue = z
            MeshExtensions.AddOrigin(_group, 0.5);  // black

            // Make the ground.
            const double groundY = -5;
            MakeGround(groundY);

            // Various robot dimensions.
            const double headR = 1.5;           // Head radius.
            const double neckLen = headR;       // Neck length.
            const double backLen = 3 * headR;   // Back length.
            const double shouW = 3 * headR;     // Shoulder width.
            const double uaLen = 2 * headR;     // Upper arm length.
            const double laLen = 2 * headR;     // Lower arm length
            const double hipsW = 2 * headR;     // Hip width.
            const double ulLen = 2 * headR;     // Upper leg length.
            const double llLen = 2 * headR;     // Lower leg length.
            const double boneR = 0.3;           // Bone radius.
            const double jointR = 0.4;          // Joint radius.
            const double height = 2 * headR + neckLen + backLen + ulLen + llLen;
            const double headY = height - headR;    // Distance from center of head to ground.
            Brush boneBrush = Brushes.PowderBlue;

            // This group represents the whole robot.
            _groupRobot = new Model3DGroup();
            _group.Children.Add(_groupRobot);
            _groupRobot.Transform = new TranslateTransform3D(0, headY + groundY, 0);

            // Head.
            // Skull.
            MeshGeometry3D skullMesh = new MeshGeometry3D();
            skullMesh.AddSphere(D3.Origin, headR, 20, 10, true);
            GeometryModel3D skullModel = skullMesh.MakeModel(boneBrush);

            // Nose.
            MeshGeometry3D noseMesh = new MeshGeometry3D();
            Point3D noseCenter = new Point3D(0, 0, headR);
            Point3D[] nosePoints = G3.MakePolygonPoints(10, noseCenter, D3.XVector(headR * 0.2), D3.YVector(headR * 0.2));
            Vector3D noseAxis = new Vector3D(0, 0, headR);
            noseMesh.AddConeFrustum(noseCenter, nosePoints, noseAxis, headR * 0.5);
            GeometryModel3D noseModel = noseMesh.MakeModel(Brushes.Orange);

            // Eyes and smile.
            MeshGeometry3D eyeMesh = new MeshGeometry3D();
            Point3D eyeCenter = SphericalToCartesian(headR, -Math.PI * 0.2, Math.PI * 0.4);
            eyeMesh.AddSphere(eyeCenter, headR * 0.2, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0.2, Math.PI * 0.4);
            eyeMesh.AddSphere(eyeCenter, headR * 0.2, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0, Math.PI * 0.7);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0.1, Math.PI * 0.67);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, -Math.PI * 0.1, Math.PI * 0.67);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0.15, Math.PI * 0.6);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, -Math.PI * 0.15, Math.PI * 0.6);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            GeometryModel3D eyeModel = eyeMesh.MakeModel(Brushes.Black);

            // Hat.
            MeshGeometry3D hatMesh = new MeshGeometry3D();
            Point3D hatCenter = new Point3D(0, headR * 0.75, 0);
            hatMesh.AddSphere(hatCenter, headR * 0.75, 20, 10, true);
            const double hatR = headR * 1.2;
            Point3D[] hatPgon = G3.MakePolygonPoints(20, hatCenter, D3.XVector(hatR), D3.ZVector(hatR));
            hatMesh.AddCylinder(hatPgon, D3.YVector(-0.2), true);

            GeometryModel3D hatModel = hatMesh.MakeModel(Brushes.SaddleBrown);

            // Head groups.
            _groupHead = JoinBones(_groupRobot, null);
            _groupHead.Children.Add(skullModel);
            _groupHead.Children.Add(noseModel);
            _groupHead.Children.Add(eyeModel);
            _groupHead.Children.Add(hatModel);

            // Neck.
            MeshGeometry3D neckMesh = new MeshGeometry3D();
            Point3D[] neckPgon = G3.MakePolygonPoints(10, D3.Origin, D3.XVector(boneR), D3.ZVector(boneR));
            neckMesh.AddCylinder(neckPgon, D3.YVector(-neckLen), true);
            GeometryModel3D neckModel = neckMesh.MakeModel(boneBrush);

            _groupNeck = JoinBones(_groupHead, new TranslateTransform3D(0, -headR, 0));
            _groupNeck.Children.Add(neckModel);

            // Shoulders.
            MeshGeometry3D shoulderMesh = new MeshGeometry3D();
            Point3D[] shouldersPgon = G3.MakePolygonPoints(10, new Point3D(-shouW / 2, 0, 0), D3.ZVector(boneR), D3.YVector(-boneR));
            shoulderMesh.AddCylinder(shouldersPgon, D3.XVector(shouW), true);
            GeometryModel3D shoulderModel = shoulderMesh.MakeModel(boneBrush);

            _groupShoulder = JoinBones(_groupNeck, new TranslateTransform3D(0, -neckLen, 0));
            _groupShoulder.Children.Add(shoulderModel);

            // Left upper arm.
            MeshGeometry3D luArmMesh = new MeshGeometry3D();
            luArmMesh.AddCylinder(neckPgon, D3.YVector(-uaLen), true);
            luArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D luArmModel = luArmMesh.MakeModel(boneBrush);

            _groupLeftUpperArm = JoinBones(_groupShoulder, new TranslateTransform3D(shouW / 2, 0, 0));
            _groupLeftUpperArm.Children.Add(luArmModel);

            // Right upper arm.
            MeshGeometry3D ruArmMesh = new MeshGeometry3D();
            ruArmMesh.AddCylinder(neckPgon, D3.YVector(-uaLen), true);
            ruArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D ruArmModel = ruArmMesh.MakeModel(boneBrush);

            _groupRightUpperArm = JoinBones(_groupShoulder, new TranslateTransform3D(-shouW / 2, 0, 0));
            _groupRightUpperArm.Children.Add(ruArmModel);

            // Left lower arm.
            MeshGeometry3D llArmMesh = new MeshGeometry3D();
            llArmMesh.AddCylinder(neckPgon, D3.YVector(-laLen), true);
            llArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D llArmModel = llArmMesh.MakeModel(boneBrush);

            _groupLeftLowerArm = JoinBones(_groupLeftUpperArm, new TranslateTransform3D(0, -uaLen, 0));
            _groupLeftLowerArm.Children.Add(llArmModel);

            // Right lower arm.
            MeshGeometry3D rlArmMesh = new MeshGeometry3D();
            rlArmMesh.AddCylinder(neckPgon, D3.YVector(-laLen), true);
            rlArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D rlArmModel = rlArmMesh.MakeModel(boneBrush);

            _groupRightLowerArm = JoinBones(_groupRightUpperArm, new TranslateTransform3D(0, -uaLen, 0));
            _groupRightLowerArm.Children.Add(rlArmModel);

            // Back and hips.
            MeshGeometry3D backMesh = new MeshGeometry3D();
            backMesh.AddCylinder(neckPgon, D3.YVector(-backLen), true);
            GeometryModel3D backModel = backMesh.MakeModel(boneBrush);

            MeshGeometry3D hipsMesh = new MeshGeometry3D();
            Point3D[] hipsPgon = G3.MakePolygonPoints(10, new Point3D(-hipsW / 2, -backLen, 0), D3.ZVector(boneR), D3.YVector(-boneR));
            hipsMesh.AddCylinder(hipsPgon, D3.XVector(hipsW), true);
            GeometryModel3D hipsModel = hipsMesh.MakeModel(boneBrush);

            _groupBack = JoinBones(_groupNeck, new TranslateTransform3D(0, -neckLen, 0));
            _groupBack.Children.Add(backModel);
            _groupBack.Children.Add(hipsModel);

            // Left upper leg.
            MeshGeometry3D luLegMesh = new MeshGeometry3D();
            luLegMesh.AddCylinder(neckPgon, D3.YVector(-ulLen), true);
            luLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D luLegModel = luLegMesh.MakeModel(boneBrush);

            _groupLeftUpperLeg = JoinBones(_groupBack, new TranslateTransform3D(-hipsW / 2, -backLen, 0));
            _groupLeftUpperLeg.Children.Add(luLegModel);

            // Right upper leg.
            MeshGeometry3D ruLegMesh = new MeshGeometry3D();
            ruLegMesh.AddCylinder(neckPgon, D3.YVector(-ulLen), true);
            ruLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D ruLegModel = ruLegMesh.MakeModel(boneBrush);

            _groupRightUpperLeg = JoinBones(_groupBack, new TranslateTransform3D(hipsW / 2, -backLen, 0));
            _groupRightUpperLeg.Children.Add(ruLegModel);

            // Left lower leg.
            MeshGeometry3D llLegMesh = new MeshGeometry3D();
            llLegMesh.AddCylinder(neckPgon, D3.YVector(-llLen), true);
            llLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D llLegModel = llLegMesh.MakeModel(boneBrush);

            _groupLeftLowerLeg = JoinBones(_groupLeftUpperLeg, new TranslateTransform3D(0, -ulLen, 0));
            _groupLeftLowerLeg.Children.Add(llLegModel);

            // Right lower leg.
            MeshGeometry3D rlLegMesh = new MeshGeometry3D();
            rlLegMesh.AddCylinder(neckPgon, D3.YVector(-llLen), true);
            rlLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D rlLegModel = rlLegMesh.MakeModel(boneBrush);

            _groupRightLowerLeg = JoinBones(_groupRightUpperLeg, new TranslateTransform3D(0, -ulLen, 0));
            _groupRightLowerLeg.Children.Add(rlLegModel);
        }

        /// Join two bones together.
        Model3DGroup JoinBones(Model3DGroup parentGroup, Transform3D offset)
        {
            Model3DGroup offsetGroup = new Model3DGroup();
            offsetGroup.Transform = offset;
            parentGroup.Children.Add(offsetGroup);

            Model3DGroup result = new Model3DGroup();
            offsetGroup.Children.Add(result);
            return result;
        }

        /// Make the ground mesh.
        void MakeGround(double groundY)
        {
            MeshGeometry3D groundMesh = new MeshGeometry3D();
            const double dx = 15;
            const double dy = 1;
            const double dz = dx;
            Point3D corner = new Point3D(-dx / 2, groundY - dy, -dz / 2);
            groundMesh.AddBoxWrapped(corner, D3.XVector(dx), D3.YVector(dy), D3.ZVector(dz));

            Point[] topCoords =
            {
                new Point(0.1, 0.1),
                new Point(0.1, 0.9),
                new Point(0.9, 0.9),
                new Point(0.9, 0.1),
            };
            Point[] frontCoords =
            {
                new Point(0.0, 0.1),
                new Point(0.0, 0.9),
                new Point(0.1, 0.9),
                new Point(0.1, 0.1),
            };
            Point[] leftCoords =
            {
                new Point(0.9, 0.0),
                new Point(0.1, 0.0),
                new Point(0.1, 0.1),
                new Point(0.9, 0.1),
            };
            Point[] rightCoords =
            {
                new Point(0.1, 1.0),
                new Point(0.9, 1.0),
                new Point(0.9, 0.9),
                new Point(0.1, 0.9),
            };
            Point[] backCoords =
            {
                new Point(1.0, 0.9),
                new Point(1.0, 0.1),
                new Point(0.9, 0.1),
                new Point(0.9, 0.9),
            };
            Point[] bottomCoords =
            {
                new Point(0.9, 0.1),
                new Point(0.9, 0.9),
                new Point(0.1, 0.9),
                new Point(0.1, 0.1),
            };

            groundMesh.AddBox(corner, D3.XVector(dx), D3.YVector(dy), D3.ZVector(dz),
                frontCoords, leftCoords, rightCoords, backCoords, topCoords, bottomCoords);

            _group.Children.Add(groundMesh.MakeModel(System.IO.Path.Combine(_resDir, @"rock.jpg")));
        }

        void neckSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupNeck.Transform = D3.Rotate(D3.YVector(), D3.Origin, neckSlider.Value);
        }

        void leftShoulderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupLeftUpperArm.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftShoulderSlider.Value);
        }

        void rightShoulderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupRightUpperArm.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightShoulderSlider.Value);
        }

        void leftElbowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
//            _groupLeftLowerArm.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftElbowSlider.Value);
        }

        void rightElbowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupRightLowerArm.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightElbowSlider.Value);
        }

        void leftHipSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupLeftUpperLeg.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftHipSlider.Value);
        }

        void rightHipSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupRightUpperLeg.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightHipSlider.Value);
        }

        void leftKneeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupLeftLowerLeg.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftKneeSlider.Value);
        }

        void rightKneeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _groupRightLowerLeg.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightKneeSlider.Value);
        }

        /// Convert from spherical to Cartesian coordinates.
        Point3D SphericalToCartesian(double r, double theta, double phi)
        {
            double y = r * Math.Cos(phi);
            double h = r * Math.Sin(phi);
            double x = h * Math.Sin(theta);
            double z = h * Math.Cos(theta);
            return new Point3D(x, y, z);
        }
        #endregion

        #region Garden
        /// Define the model.
        void DefineModelGarden()
        {
            Width = 700;
            Height = 700;

            // Rock sections.
            MeshGeometry3D rockMesh = new MeshGeometry3D();
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

            ImageBrush rockBrush = new ImageBrush();
            rockBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"rocks.jpg"), UriKind.Relative));
            Material rockMaterial = new DiffuseMaterial(rockBrush);
            GeometryModel3D rockModel = new GeometryModel3D(rockMesh, rockMaterial);
            _group.Children.Add(rockModel);

            // Grass sections.
            MeshGeometry3D grassMesh = new MeshGeometry3D();
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

            ImageBrush grassBrush = new ImageBrush();
            grassBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"grass.jpg"), UriKind.Relative));
            Material grassMaterial = new DiffuseMaterial(grassBrush);
            GeometryModel3D grassModel = new GeometryModel3D(grassMesh, grassMaterial);
            _group.Children.Add(grassModel);

            // Water.
            MeshGeometry3D waterMesh = new MeshGeometry3D();
            AddRectangle(waterMesh,
                new Point3D(-1, 0, -1),
                new Point3D(-1, 0, +1),
                new Point3D(+1, 0, +1),
                new Point3D(+1, 0, -1));
            ImageBrush waterBrush = new ImageBrush();
            waterBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"water.jpg"), UriKind.Relative));
            Material waterMaterial = new DiffuseMaterial(waterBrush);
            GeometryModel3D waterModel = new GeometryModel3D(waterMesh, waterMaterial);
            _group.Children.Add(waterModel);

            // Cube brick face.
            MeshGeometry3D brickMesh = new MeshGeometry3D();
            AddRectangle(brickMesh,
                new Point3D(-1, 2, -1),
                new Point3D(-1, 0, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 2, -1));
            ImageBrush brickBrush = new ImageBrush();
            brickBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"bricks.jpg"), UriKind.Relative));
            Material brickMaterial = new DiffuseMaterial(brickBrush);
            GeometryModel3D brickModel = new GeometryModel3D(brickMesh, brickMaterial);
            _group.Children.Add(brickModel);

            // Cube metal face.
            MeshGeometry3D metalMesh = new MeshGeometry3D();
            AddRectangle(metalMesh,
                new Point3D(+1, 2, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 0, -3),
                new Point3D(+1, 2, -3));
            ImageBrush metalBrush = new ImageBrush();
            metalBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"metal.jpg"), UriKind.Relative));
            Material metalMaterial = new DiffuseMaterial(metalBrush);
            GeometryModel3D metalModel = new GeometryModel3D(metalMesh, metalMaterial);
            _group.Children.Add(metalModel);

            // Cube wood face.
            MeshGeometry3D woodMesh = new MeshGeometry3D();
            AddRectangle(woodMesh,
                new Point3D(-1, 2, -3),
                new Point3D(-1, 2, -1),
                new Point3D(+1, 2, -1),
                new Point3D(+1, 2, -3));
            ImageBrush woodBrush = new ImageBrush();
            woodBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"wood.jpg"), UriKind.Relative));
            Material woodMaterial = new DiffuseMaterial(woodBrush);
            GeometryModel3D woodModel = new GeometryModel3D(woodMesh, woodMaterial);
            _group.Children.Add(woodModel);

            // Cube fire face.
            MeshGeometry3D fireMesh = new MeshGeometry3D();
            AddRectangle(fireMesh,
                new Point3D(-1, 2, -3),
                new Point3D(-1, 0, -3),
                new Point3D(-1, 0, -1),
                new Point3D(-1, 2, -1));
            ImageBrush fireBrush = new ImageBrush();
            fireBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"fire.jpg"), UriKind.Relative));
            Material fireMaterial = new DiffuseMaterial(fireBrush);
            GeometryModel3D fireModel = new GeometryModel3D(fireMesh, fireMaterial);
            _group.Children.Add(fireModel);

            // Cube cloth face.
            MeshGeometry3D clothMesh = new MeshGeometry3D();
            AddRectangle(clothMesh,
                new Point3D(+1, 2, -3),
                new Point3D(+1, 0, -3),
                new Point3D(-1, 0, -3),
                new Point3D(-1, 2, -3));
            ImageBrush clothBrush = new ImageBrush();
            clothBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"cloth.jpg"), UriKind.Relative));
            Material clothMaterial = new DiffuseMaterial(clothBrush);
            GeometryModel3D clothModel = new GeometryModel3D(clothMesh, clothMaterial);
            _group.Children.Add(clothModel);

            // Skybox meshes.
            MeshGeometry3D sky1Mesh = new MeshGeometry3D();
            AddRectangle(sky1Mesh,
                new Point3D(-6, +7, +6),
                new Point3D(-6, -5, +6),
                new Point3D(-6, -5, -6),
                new Point3D(-6, +7, -6));
            ImageBrush sky1Brush = new ImageBrush();
            sky1Brush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"clouds.jpg"), UriKind.Relative));
            MaterialGroup sky1Group = new MaterialGroup();
            sky1Group.Children.Add(new DiffuseMaterial(sky1Brush));
            sky1Group.Children.Add(new EmissiveMaterial(new SolidColorBrush(
                Color.FromArgb(255, 128, 128, 128))));
            GeometryModel3D sky1Model = new GeometryModel3D(sky1Mesh, sky1Group);
            _group.Children.Add(sky1Model);

            MeshGeometry3D sky2Mesh = new MeshGeometry3D();
            AddRectangle(sky2Mesh,
                new Point3D(-6, +7, -6),
                new Point3D(-6, -5, -6),
                new Point3D(+6, -5, -6),
                new Point3D(+6, +7, -6));
            ImageBrush sky2Brush = new ImageBrush();
            sky2Brush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"clouds.jpg"), UriKind.Relative));
            MaterialGroup sky2Group = new MaterialGroup();
            sky2Group.Children.Add(new DiffuseMaterial(sky2Brush));
            sky2Group.Children.Add(new EmissiveMaterial(new SolidColorBrush(
                Color.FromArgb(255, 64, 64, 64))));
            GeometryModel3D sky2Model = new GeometryModel3D(sky2Mesh, sky2Group);
            _group.Children.Add(sky2Model);

            MeshGeometry3D sky3Mesh = new MeshGeometry3D();
            AddRectangle(sky3Mesh,
                new Point3D(-6, -5, +6),
                new Point3D(+6, -5, +6),
                new Point3D(+6, -5, -6),
                new Point3D(-6, -5, -6));
            ImageBrush sky3Brush = new ImageBrush();
            sky3Brush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(_resDir, @"clouds.jpg"), UriKind.Relative));
            Material sky3Material = new DiffuseMaterial(sky3Brush);
            GeometryModel3D sky3Model = new GeometryModel3D(sky3Mesh, sky3Material);
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
        #endregion
    }
}
