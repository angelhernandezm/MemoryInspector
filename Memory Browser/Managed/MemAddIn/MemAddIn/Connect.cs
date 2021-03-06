// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : Connect.cs
   Framework version: .NET Framework version 4.0
   C# compiler      : Microsoft (R) Visual C# 2010 Compiler version 4.0.30319.1
   Creation date    : 30/03/2011
   Developer        : Angel Hernández Matos
   e-m@il           : angel@bonafideideas.com 
				    : angeljesus14@hotmail.com
   Website          : http://www.bonafideideas.com
*/

using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using MemAddIn.Code;
using MemAddIn.UI;
using MemAddIn.UI.Windows;


namespace MemAddIn {
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget {
		#region "Consts"

		private const string WINDOW_TITLE = "Memory Inspector";
		private const string TOOL_WINDOW_GUID = "{B465BD0C-B622-4AB8-A35C-B8835C0E6E2D}";
		private const string TOOL_WINDOW_CREATION_FAILURE = "Failed to create Tool window user control";

		#endregion

		#region "Members"

		private Window _toolWindow;
		private AddIn _addInInstance;
		private DTE2 _applicationObject;
		private MainToolWindow _toolWindowControl;

		#endregion



		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect() {
		}

		private void CreateToolWindow() {
			object tempControl = null;
			Windows2 applicationWindow = (Windows2)_applicationObject.Windows;

			if (_toolWindow == null) {
				_toolWindow = applicationWindow.CreateToolWindow2(_addInInstance, GetType().Assembly.Location,
					typeof(MainToolWindow).FullName, WINDOW_TITLE, TOOL_WINDOW_GUID, ref tempControl);
				if ((_toolWindowControl = tempControl as MainToolWindow) != null) {
					_toolWindowControl.Application = _applicationObject;
					_toolWindowControl.Visible = _toolWindow.Visible = true;
				} else
					throw new Exception(TOOL_WINDOW_CREATION_FAILURE);
			} else
				ShowToolWindow();

			LaunchExternalWPFWindow();
		}


		/// <summary>
		/// Launches the external WPF window.
		/// </summary>
		private void LaunchExternalWPFWindow() {
			_toolWindowControl.wpfWindowHwnd = IntPtr.Zero;
		}

		/// <summary>
		/// Shows the tool window.
		/// </summary>
		private void ShowToolWindow() {
			if (_toolWindow == null)
				CreateToolWindow();
			else
				_toolWindow.Visible = true;
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
			if (connectMode == ext_ConnectMode.ext_cm_UISetup) {
				object[] contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try {
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(_addInInstance, "MemAddIn", "Memory Inspector AddIn", "Executes the command for MemAddIn", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if ((command != null) && (toolsPopup != null)) {
						command.AddControl(toolsPopup.CommandBar, 1);
					}
				} catch (System.ArgumentException) {
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
					//  safely ignore the exception.
				}
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) {
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom) {
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom) {
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom) {
		}

		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText) {
			if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone) {
				if (commandName == "MemAddIn.Connect.MemAddIn") {
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled) {
			handled = false;
			if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault) {
				if (commandName == "MemAddIn.Connect.MemAddIn") {
					CreateToolWindow();
					handled = true;
					return;
				}
			}
		}
	}
}