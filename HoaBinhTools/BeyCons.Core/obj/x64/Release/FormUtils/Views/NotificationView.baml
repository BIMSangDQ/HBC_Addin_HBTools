   M S B A M L   `   `   `  ���� G  CBeyCons.Core, Version=2.5.7.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BeyCons.Core.FormUtils.ViewsBeyCons.Core.FormUtils.Views  3    -BeyCons.Core.FormUtils.Views.NotificationView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     <local*clr-namespace:BeyCons.Core.FormUtils.Views     -     �Name$  wdNotification��$	��500��$
��Auto�� � Title$ Notify Revit�� � SizeToContent$ Height=� � 
ResizeMode$ NoResize=� � Topmost .  � WindowStartupLocation$ CenterScreen=� J� InputBindings �� 
 �� Key$	 Esc�� �� Command �� *
ESCCommand+	 �� CommandParameter	 �� 
 �� ElementName$
 wdNotification��.����  �� Padding$ 
10 0 10 10q�._���  �� Orientation$ Vertical=�.+��  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��Notification��$
��Left=�$��Center=� �� Padding$ 0 0 5 5q�$
��Bold"��� $��1��$	��200�� �� 
IsReadOnly . $	��0 5q�$	��Top=�$��Stretch=�$
��Auto=�. �/�  /� TextAlignment$
 Left=�.T�J� .*�.*��� ��� *	Content+�� $��2��$��Close��$
��Bold"�$��Center=�$��Right=�$	��100��$��5q�$��0 10 0 0q�y��� *VisibilityClose+���� *CloseCommand+���� $
 wdNotification���� $ 
Horizontal=�$��Right=�$��3��.+��� $	��Yes��$
��Bold"�$��Center=�$��Right=�$	��100��$��5q�$��0 10 5 0q�y��� *VisibilityYesNo+���� *
YesCommand+���� $
 wdNotification���� $��No��$
��Bold"�$��Center=�$��Right=�$	��100��$��5q�$��5 10 0 0q�y��� *VisibilityYesNo+���� *	NoCommand+���� $
 wdNotification��