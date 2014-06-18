using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using FurtherMath.Base.Collections;

using WaveComparer.Lib.Interfaces;

namespace WaveComparer.Lib.Event_Args
{
    public class ChangedEventManager : WeakEventManager
    {
        public static ChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(ChangedEventManager);
                var manager = WeakEventManager.GetCurrentManager(managerType) as ChangedEventManager;

                if (manager == null)
                {
                    manager = new ChangedEventManager();
                    WeakEventManager.SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        public static void AddListener(INotifyChanged source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(INotifyChanged source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        protected override void StartListening(object source)
        {
            ((INotifyChanged)source).Changed += this.DeliverEvent;            
        }

        protected override void StopListening(object source)
        {
            ((INotifyChanged)source).Changed += this.DeliverEvent;
        }
    }
}
