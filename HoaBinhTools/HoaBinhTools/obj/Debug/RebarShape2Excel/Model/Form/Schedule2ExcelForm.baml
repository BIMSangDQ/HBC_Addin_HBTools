   M S B A M L   `   `   `  ����G  CHoaBinhTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Model.Form
Model.Form  5      +clr-namespace:Property.WPFProperty.WPF  5      #    Model.Form.Schedule2ExcelForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      *localclr-namespace:Model.Form     5      /propertyclr-namespace:Property.WPF     5        � SizeToContent$  WidthAndHeight=�5	       � 
ResizeMode$ NoResize=�6-    � Title$ Rebar Schedule To Excel��6C    � 	Resources 5
       �� Source�� 5   
   $ ..\Style\Styles.xaml?�6   %    .��5      ��  �� Orientation$ Vertical=�6   .+�5   
   �� $��File Options��6   $��5q�6)   .��5      �� $ Vertical=�6   $��5q�60   .+�5      � $��5q�6    � ColumnDefinitions 5      �� 5      $
��auto �6+   �� 5      $��* �6+   �� 5      $
��auto �6+   .+�5      �� $��0��6   $��Save As:��6,   $��DemiBold"�6?   �� 5      $��1��6   $��Stretch=�6H   $��Right=�6f   $��	10 0 30 0q�6�   $��Center=�5      {�.  ��5   .   �� *
SavePath+5   <   �� 5      $��2��6      btnStandard#��[  6-   $��50��6R   $��Select��6]   '   !Property.WPF.BaseAttachedProperty    BrowseFileClickedProperty$
 true��5      5      �� 5      $��
35,20,0,10q�6   $"��Open excel file after export��6/    d� 	IsChecked 6V   �� *IsOpenExcel+5      5      �� 5   
   $��Rebar Schedule��6   $��5q�6+   .��5       �  5!      �� 5"      $��* �6'   �� 5#      $
��auto �6'   �� 5$      $��* �6'   .+�5&      �� $ Vertical=�6   $��5q�64   $��0��6?   .+�5'      �� $��Available Schedules��6   $��	15 0 0 15q�6:   $��DemiBold"�6M   �� 5(      -   5        	 ��Name$	 availableScheduleListView��5(      
 �� DisplayMemberPath$

 Name��5)      $	��350��5+      $	��400��5,      $��Visible=�5-   !   ��. ��5(   B   �� *ViewSchedules+ �� SelectedItem 5*      �� *SelectedViewSchedule+5-   {   5.      �� 5/      $��10q�6   $��1��6)   $ Vertical=�69   $��Stretch=�6P   $��Center=�6n   .+�50      ��   imageMediumButton#��[ 6   $   AddLeftToRightClickedProperty$
 true��6H   $��Center=�51   !   $��70��6>   $��30��6I   $��0 0 0 12q�6U   .��52      �� $��1q�6    $.��(/Schedule2Excel;component/Image/next.png��6+   53      �� 54      #��[ 6   $   AddRightToLeftClickedProperty$
 true��6H   $��Center=�55   !   $��70��6>   $��30��6I   $��0 0 0 12q�6U   .��56      �� $��1q�6    $.��(/Schedule2Excel;component/Image/back.png��6+   57      �� 58      #��[  6   $��Export��6B   $��Center=�6S   $��70��6p   $��30��6{      ExportClickedProperty$
 true��59   !   5:      �� 5;      $ Vertical=�6   $��5q�64   $��2��6?   .+�5<      �� $��Export Schedules��6   $��	15 0 0 15q�67   $��DemiBold"�6J   �� 5=      -   5        $	 exportScheduleListView��5=      $	��500��5>      $��Visible=�6/   ��. $

 Name��5@      ��5?      �� *ExportViewSchedules+ 5A      �� *SelectedExportViewSchedule+��5B      ��  �� ElementName$ availableScheduleListView�� �� Path$ ActualHeight���5C      �� $ availableScheduleListView��$ ActualWidth�6i   5D      5E      5F      �� 5G   
   #��[  6   $��Close��66   $��50��6F   $��Right=�6Q   $��	0 7 15 15q�5H         CloseClickedProperty$
 true��5I      5J      5�      