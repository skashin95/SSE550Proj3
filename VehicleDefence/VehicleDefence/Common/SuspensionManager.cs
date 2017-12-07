using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VehicleDefence.Common
{
    internal sealed class SuspensionManager
    {
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();
        private static List<Type> _knownTypes = new List<Type>();
        private const string sessionStateFilename = "_sessionState.xml";

        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        public static List<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        public static async Task SaveAsync()
        {
            try
            {
                foreach(var weakFrameReference in _registeredFrames)
                {
                    weakFrameReference frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNaviationState(frame);
                    }
                }

                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string,object>, _knownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using(Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
            catch (Exception)
            {
                throw new SuspensionManagerException(e);
            }
        }

        public static async Task RestoreAsync()
        {
            _sessionState = new Dictionary<string, object>();

            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                using(IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    DataContractSerializer serialzer = new DataContractSerializer(typeof(Dictionary<string,object), _knownTypes);
                    _sessionState = (Dictionary<string,object>)serialzer.ReadObject(inStream.AsStreamForRead());
                }

                foreach(var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if(weakFrameReference.TryGetTarget(out frame))  
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        private static DependencyProperty FrameSessionStateKeyProperty = DependencyProperty.RegisterAttached("_FrameSesssionStateKey", typeof(String), typeof(SuspensionManager), null);
        private static DependencyProperty FrameSessionStateProperty = DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(SuspensionManager), null);
        private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();

        public static void RegistredFrame(Frame frame, String sessionStateKey)
        {
            if(frame.GetValue(FrameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames registered to one session state key.");
            }
            if (frame.GetValue(FrameSessionStateProperty) != null)
            {
                throw new InvalidOperationException("Frame must be registered first. then the frame state can be accessed.");
            }

            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            _registeredFrames.Add(new WeakReference<Frame>(frame));

            RestoreFrameNavigationState(frame);
        }

        public static void UnregistredFrame(Frame frame)
        {
            SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
            _registeredFrames.RemoveAll((WeakReference) =>
            {
                Frame testFrame;
                return !WeakReference.TryGetTarget(out testFrame) || testFrame == frame;
            });
        }

        public static Dictionary<String, object> SessionStateForFrame(Frame frame)
        {
            var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);

            if (frameState == null)
            {
                var frameSesionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
                if(frameSesionKey != null)
                {
                    if(!_sessionState.ContainsKey(frameSesionKey))
                    {
                        _sessionState[frameSesionKey] = new Dictionary<String, Object>();
                    }
                    frameState = (Dictionary<String, Object>)_sessionState[frameSesionKey];
                }
                else
                {
                    frameState = new Dictionary<string, object>();
                }
                frame.SetValue(FrameSessionStateProperty, frameState);
            }
            return frameState;
        }

        private static void SaveFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState["Navigation"] = frame.GetNavigationState();
        }
    }

    public class SuspensionManagerException : Exception
    {
        public SuspensionManagerException()
        {

        }

        public SuspensionManagerException(Exception e) : base("SuspensionManager failed", e)
        { 

        }
    }
}
