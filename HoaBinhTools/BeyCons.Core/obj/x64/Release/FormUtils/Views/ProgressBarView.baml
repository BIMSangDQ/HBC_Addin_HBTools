   M S B A M L   `   `   `  ���� G  CBeyCons.Core, Version=2.5.7.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BeyCons.Core.FormUtils.ViewsBeyCons.Core.FormUtils.Views  2    ,BeyCons.Core.FormUtils.Views.ProgressBarView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     <local*clr-namespace:BeyCons.Core.FormUtils.Views     $
��Auto��$	��500��  � 
ResizeMode$  NoResize=� � WindowStartupLocation$ CenterScreen=� � Title$ Notify from Revit ...�� � SizeToContent$ Height=� � Topmost . .����  �� Padding$ 
10 0 10 10q�._��  � ColumnDefinitions �� $��* � � RowDefinitions �� $
��Auto ��� $
��Auto �.+��� $��0�� �� Padding$ 0 0 3 10q�$
��Bold"����� 	 �� Path$	 Status�� -   
 �Name$
 ProgressBar��$��1��$��25�� � Minimum$ 0Q� � Maximum$	 100Q��� $��1��$��Center=�$��Center=�$
��Bold"����� $	 Ratio�