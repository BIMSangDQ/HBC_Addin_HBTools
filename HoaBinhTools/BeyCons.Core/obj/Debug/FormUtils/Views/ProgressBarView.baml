   M S B A M L   `   `   `  ����G  CBeyCons.Core, Version=2.5.7.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BeyCons.Core.FormUtils.ViewsBeyCons.Core.FormUtils.Views  5      2    ,BeyCons.Core.FormUtils.Views.ProgressBarView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:BeyCons.Core.FormUtils.Views     5      $
��Auto��5      $	��500��6     � 
ResizeMode$  NoResize=�6(    � WindowStartupLocation$ CenterScreen=�6>    � Title$ Notify from Revit ...��6c    � SizeToContent$ Height=�6�    � Topmost . .��5      ��  �� Padding$ 
10 0 10 10q�6   ._�5	   
   �  � ColumnDefinitions 5
      �� 5      $��* �6#    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5      �� $��0��6    �� Padding$ 0 0 3 10q�6D   $
��Bold"�6W   ��6%   �� 	 �� Path$	 Status�6W   � 5      -   5        
 �Name$
 ProgressBar��5   '   $��1��6   $��25��6<    � Minimum$ 0Q�6H    � Maximum$	 100Q�6T   �� 5      $��1��6   $��Center=�6C   $��Center=�6`   $
��Bold"�6{   ��6%   �� $	 Ratio�6{   5      5      5      