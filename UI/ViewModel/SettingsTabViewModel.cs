﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace UI.ViewModel
{
  public class SettingsTabViewModel : BaseViewModel
  {
    #region fields
    private string name;
    protected Dispatcher dispatcher;
    #endregion

    #region properties
    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged("Name");
      }
    }
    #endregion

    public SettingsTabViewModel(string tabName, Dispatcher dispatcher)
    {
      Name = tabName;
      this.dispatcher = dispatcher;
    }

    public virtual void SaveSettings() { }  
  }
}