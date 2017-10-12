using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.IO;

namespace faceTrackingApp
{
    class Program
    {
        public static KinectSensor _sensor = null;
        public static BodyFrameReader _bodyFrameReader = null;
        public static HighDefinitionFaceFrameSource _faceSource = null;
        public static HighDefinitionFaceFrameReader _faceReader = null;
        public static FaceAlignment _faceAlignment = null;
        public static FaceModel _faceModel = null;
        public static CoordinateMapper _coordinateMapper = null;
        public static Worker _worker = new Worker();
        public static FileStream _animationUnitStream, _verticesFileStream;
        public static bool trackingSuccess = false;

        static void Main(string[] args)
        {
            _sensor = KinectSensor.GetDefault();

            _worker.getSubjectID();

            if (_sensor != null)
            {
                _sensor.Open();
                Console.WriteLine("sensorOpened");
                if (_sensor.IsOpen)
                {
                    _coordinateMapper = _sensor.CoordinateMapper;
                    _bodyFrameReader = _sensor.BodyFrameSource.OpenReader();

                    _bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;

                    _faceSource = new HighDefinitionFaceFrameSource(_sensor);
                    _faceReader = _faceSource.OpenReader();
                    _faceReader.FrameArrived += FaceReader_FrameArrived;

                    _faceModel = new FaceModel();
                    _faceAlignment = new FaceAlignment();

                }
            }
            string input = Console.ReadLine();
            _sensor.Close();
        }

        private static void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrame _bodyFrame = e.FrameReference.AcquireFrame();
            if (_bodyFrame == null)
            {
                return;
            }

            if (_worker.counterFile == 0 && _worker.counterFrame == 0)
            {
                _worker.CreateFolders();
                _worker.InilizeAnimationUnitStream();
            }

            _bodyFrame.GetAndRefreshBodyData(_worker._bodies);

            Body body = _worker._bodies.Where(b => b.IsTracked).FirstOrDefault();

            if (!_faceSource.IsTrackingIdValid)
            {
                if (body != null)
                {
                    _faceSource.TrackingId = body.TrackingId;
                }
            }

        }

        static void FaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            TimeSpan milliseconds = DateTime.Now.TimeOfDay;
            using (var _faceFrame = e.FrameReference.AcquireFrame())
            {

                if (_faceFrame != null && _faceFrame.IsFaceTracked)
                {
                    if (!trackingSuccess)
                    {
                        Console.WriteLine("started face tracking..");
                        trackingSuccess = true;
                    }
                    _faceFrame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                    Task.Factory.StartNew(() =>
                    {
                        _worker.WriteAnimationUnits(_faceAlignment, milliseconds);
                    });
                }
                else
                {
                    if (trackingSuccess)
                    {
                        Console.WriteLine("tracking is lost..");
                        trackingSuccess = false;
                    }
                    _worker.WriteAnimationUnits(_faceAlignment, milliseconds, false);
                }
            }

        }
    }
}
