﻿namespace BmLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys      

        /// <summary>
        /// Gets the color key for the normal control enabled background color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");


        /// <summary>
        /// Gets a the applicable foreground Brush key that should be used for coloring text.
        /// </summary>
        public static readonly ComponentResourceKey ControlTextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlTextBrushKey");

        /// <summary>
        /// Gets the Brush key for the normal control enabled foreground color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundBrushKey");

        /// <summary>
        /// Gets the Brush key of the border color of a control.
        /// </summary>
        public static readonly ComponentResourceKey ControlBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlBorderBrushKey");

        #region Arrow BrushKeys
        /// <summary>
        /// Gets the Brush key that defines the outline of a glyph (Path.Stroke).
        /// </summary>
        public static readonly ComponentResourceKey ArrowBorderForeground = new ComponentResourceKey(typeof(ResourceKeys), "ArrowBorderForeground");

        /// <summary>
        /// Gets the Brush key that defines the fill color of a glyph (Path.Fill).
        /// </summary>
        public static readonly ComponentResourceKey ArrowFillForeground = new ComponentResourceKey(typeof(ResourceKeys), "ArrowFillForeground");

        /// <summary>
        /// Gets the mouse over Brush key that defines the fill color of a glyph (Path.Fill).
        /// </summary>
        public static readonly ComponentResourceKey ArrowFillMouseOverForeground = new ComponentResourceKey(typeof(ResourceKeys), "ArrowFillMouseOverForeground");

        /// <summary>
        /// Gets the mouse over Brush key that defines the outline of a glyph (Path.Stroke).
        /// </summary>
        public static readonly ComponentResourceKey ArrowBorderMouseOverForeground = new ComponentResourceKey(typeof(ResourceKeys), "ArrowBorderMouseOverForeground");

        /// <summary>
        /// Gets the button checked Brush key that defines the outline of a glyph (Path.Stroke).
        /// </summary>
        public static readonly ComponentResourceKey ArrowBorderCheckedForeground = new ComponentResourceKey(typeof(ResourceKeys), "ArrowBorderCheckedForeground");

        /// <summary>
        /// Gets the button checked Brush key that defines the fill color of a glyph (Path.Fill).
        /// </summary>
        public static readonly ComponentResourceKey ArrowFillCheckedForeground = new ComponentResourceKey(typeof(ResourceKeys), "ArrowFillCheckedForeground");
        #endregion Arrow BrushKeys

        /// <summary>
        /// Currently not used (or only in uncommented lines).
        /// </summary>
        public static readonly ComponentResourceKey ButtonSelectedColor = new ComponentResourceKey(typeof(ResourceKeys), "ButtonSelectedColor");

////        /// <summary>
////        /// Currently not used (or only in uncommented lines).
////        /// </summary>
////        public static readonly ComponentResourceKey ButtonBackgoundColor = new ComponentResourceKey(typeof(ResourceKeys), "ButtonBackgoundColor");

        /// <summary>
        /// Decide whether styling is using a 3D depth effect or not
        /// </summary>
        public static readonly ComponentResourceKey ThreeDStyleBrushes = new ComponentResourceKey(typeof(ResourceKeys), "ThreeDStyleBrushes");

        #region HotTrack BrushKeys
        /// <summary>
        /// TODO: Review color definition
        /// </summary>
        public static readonly ComponentResourceKey HotTrack_SelectedBrush = new ComponentResourceKey(typeof(ResourceKeys), "HotTrack_SelectedBrush");

        /// <summary>
        /// TODO: Review color definition
        /// </summary>
        public static readonly ComponentResourceKey HotTrack_BackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "HotTrack_BackgroundBrush");

        /// <summary>
        /// TODO: Review color definition
        /// </summary>
        public static readonly ComponentResourceKey HotTrack_HighlightBrush = new ComponentResourceKey(typeof(ResourceKeys), "HotTrack_HighlightBrush");
        #endregion HotTrack BrushKeys

        /// <summary>
        /// This color is applied to the drop down button (Hottrack)
        /// when it is selected and the drop down is open.
        /// </summary>
        public static readonly ComponentResourceKey OpenDropDownButton_Background = new ComponentResourceKey(typeof(ResourceKeys), "OpenDropDownButton_Background");

        /// <summary>
        /// Gets a background Brush key of an input control.
        /// </summary>
        public static readonly ComponentResourceKey ControlInputBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlInputBackgroundKey");

        /// <summary>
        /// Gets a border Brush key of an input control.
        /// </summary>
        public static readonly ComponentResourceKey ControlInputBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlInputBorderKey");

        /// <summary>
        /// Gets the Brush key of a control's background color when it is in disabled state.
        /// </summary>
        public static readonly ComponentResourceKey ControlDisabledBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlDisabledBackgroundKey");

        /// <summary>
        /// Gets the Brush key of a control's background color when it is in disabled state.
        /// </summary>
        public static readonly ComponentResourceKey ControlDisabledBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlDisabledBorderKey");

        /// <summary>
        /// Gets the Geometry (an right arrow) that is displayed on mouse over on a Breadcrumb ToggleButton.
        /// </summary>
        public static readonly ComponentResourceKey NormalArrow = new ComponentResourceKey(typeof(ResourceKeys), "NormalArrow");

        /// <summary>
        /// Gets the Geometry (a down arrow) that is displayed on mouse click on a Breadcrumb ToggleButton.
        /// -> The button is displayed in expanded (IsChecked/IsPressed) state.
        /// </summary>
        public static readonly ComponentResourceKey ExpandedArrow = new ComponentResourceKey(typeof(ResourceKeys), "ExpandedArrow");

        /// <summary>
        /// Gets the Geometry (a double left arrow) that is displayed on the very left side of the control
        /// when the Breadcrumb is overflown (not all items fit into the path display).
        /// This arrow indicates that the left most drop down list contains additional items (the overflown items).
        /// </summary>
        public static readonly ComponentResourceKey ExpanderArrow = new ComponentResourceKey(typeof(ResourceKeys), "ExpanderArrow");
        
        /// <summary>
        /// Implements the left most arrow drop down toggle button next to the root toggle button
        /// </summary>
        public static readonly ComponentResourceKey BaseToggleButton = new ComponentResourceKey(typeof(ResourceKeys), "BaseToggleButton");
        
        /// <summary>
        /// Implements the button that displays the path string
        /// </summary>
        public static readonly ComponentResourceKey BaseButton = new ComponentResourceKey(typeof(ResourceKeys), "BaseButton");
        
        /// <summary>
        /// This button is displayed when the user makes a mouse over on a Breadcrumb ToggleButton
        /// </summary>
        public static readonly ComponentResourceKey BasicArrowButton = new ComponentResourceKey(typeof(ResourceKeys), "BasicArrowButton");
        
        /// <summary>
        /// Implements the background of the Switch control to switch content with click on background
        /// </summary>
        public static readonly ComponentResourceKey BlankButton = new ComponentResourceKey(typeof(ResourceKeys), "BlankButton");

        #region Refresh Cancel Button
        /// <summary>
        /// Defines the button style of the Refresh/Cancel button on the far right side of the Breadcrumb control.
        /// </summary>
        public static readonly ComponentResourceKey RefreshCancel_ButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "RefreshCancel_ButtonStyle");

        /// <summary>
        /// Defines the Icon that is shown for Refresh button displayed on the far right side of the Breadcrumb control.
        /// </summary>
        public static readonly ComponentResourceKey ICON_Refresh = new ComponentResourceKey(typeof(ResourceKeys), "ICON_Refresh");

        /// <summary>
        /// Defines the Icon that is shown for Close button displayed on the far right side of the Breadcrumb control.
        /// This icon is displayed when the switch is turned to show text of if there is a cancelable background
        /// task that could be cancelled with this button.
        /// </summary>
        public static readonly ComponentResourceKey ICON_Close = new ComponentResourceKey(typeof(ResourceKeys), "ICON_Close");

        /// <summary>
        /// Defines the (chevron down) icon that is shown for recent location button displayed on the right side of the Breadcrumb control.
        /// </summary>
        public static readonly ComponentResourceKey ICON_ExpandDown = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ExpandDown");
        #endregion Refresh Cancel Button

        /// <summary>
        /// Defines the style of the progress bar shown inside the Breadcrumb control.
        /// </summary>
        public static readonly ComponentResourceKey BreadcrumbProgressBarStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "BreadcrumbProgressBarStyleKey");

        /// <summary>
        /// Determines the foreground color of the ProgressBar display.
        /// </summary>
        public static readonly ComponentResourceKey HotTrackBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "HotTrackBrushKey");

        /// <summary>
        /// Defines the style of Switch control inside the Breadcrumb control.
        /// </summary>
        public static readonly ComponentResourceKey BreadcrumbSwitchStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "BreadcrumbSwitchStyleKey");
    }
}
