﻿using Model.Controller;
using Open3D.Geometry.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.UserControls
{
    public class CuboidControlVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public Cuboid SelectedCuboid
        {
            get;set;
        }

        public CuboidControlVM()
        {
            MeshObjectController.SelectedCutObjectChanged += MeshObjectController_SelectedCutObjectChanged;
            
        }

        private void MeshObjectController_SelectedCutObjectChanged(object sender, EventArgs e)
        {
            if (MeshObjectController.CutObject.GetType().IsSubclassOf(typeof(Cuboid))) 
                SelectedCuboid = MeshObjectController.CutObject;

            OnPropertyChanged("SelectedCuboid");

        }
    }
}
