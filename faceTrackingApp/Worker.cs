using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows;
using System.Runtime.InteropServices;

namespace faceTrackingApp
{
    class Worker
    {
        public string _fileName { get; set; }
        private string _folderNameRec = null;
        private string _animationUnitFolder;
        public int counterFile = 0;
        public int counterFrame = 0;
        public Body[] _bodies = new Body[6];
        private StreamWriter _animationUnitWriter = null;

        public void getSubjectID()
        {
            Console.WriteLine("enter the subject ID");
            string input = Console.ReadLine();
            _folderNameRec = Path.Combine(@"C:\NOISE_PATTERN\SAVINGS\", input);
            if (!Directory.Exists(_folderNameRec))
            {
                Directory.CreateDirectory(_folderNameRec);
                Console.WriteLine("folder is created..");
            }
        }

        public void CreateFolders()
        {
            _animationUnitFolder = Path.Combine(_folderNameRec, "AnU");
            Directory.CreateDirectory(_animationUnitFolder);
        }
        public void InilizeAnimationUnitStream()
        {
            string animationUnitFile = _animationUnitFolder + "AnU_" + counterFile + ".txt";
            FileStream animationUnitStream = new FileStream(animationUnitFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _animationUnitWriter = new StreamWriter(animationUnitStream);
        }

        public void WriteAnimationUnits(FaceAlignment _faceAlignment, TimeSpan milliseconds, bool flag = true)
        {
            _animationUnitWriter.Write("\r\n");
            _animationUnitWriter.Write("{0}, ", milliseconds);
            if (flag)
            {
                IReadOnlyDictionary<FaceShapeAnimations, float> _animationUnits = _faceAlignment.AnimationUnits;
                Dictionary<FaceShapeAnimations, float> _animationUnitsDict = new Dictionary<FaceShapeAnimations, float>();

                foreach (FaceShapeAnimations _anuType in _animationUnits.Keys)
                {
                    _animationUnitWriter.Write("{0}, ", _animationUnits[_anuType]);
                }
            }
            else
            {
                _animationUnitWriter.Write("NaN, ");
            }
            _animationUnitWriter.Flush();
        }

    }
}
