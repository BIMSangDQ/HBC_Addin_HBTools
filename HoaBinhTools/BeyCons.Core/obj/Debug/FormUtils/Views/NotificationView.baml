   M S B A M L   `   `   `  ����G  CBeyCons.Core, Version=2.5.7.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BeyCons.Core.FormUtils.ViewsBeyCons.Core.FormUtils.Views  5      3    -BeyCons.Core.FormUtils.Views.NotificationView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:BeyCons.Core.FormUtils.Views     5      -   5          �Name$  wdNotification��5      $	��500��6   $
��Auto��6)    � Title$ Notify Revit��67    � SizeToContent$ Height=�6L    � 
ResizeMode$ NoResize=�6c    � Topmost .  � WindowStartupLocation$ CenterScreen=�6�    J� InputBindings 5      �� 5	   
   
 �� Key$	 Esc��6n    �� Command 6   �� *
ESCCommand+	 �� CommandParameter	 64   �� 
 �� ElementName$
 wdNotification��6n   .��5      ��  �� Padding$ 
10 0 10 10q�6   ._�5   
   ��  �� Orientation$ Vertical=�6   .+�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� $��0��6   $��Notification��6)   $
��Left=�6=   $��Center=�6X    �� Padding$ 0 0 5 5q�6s   $
��Bold"�6�   �� 5      $��1��6   $	��200��6+    �� 
IsReadOnly . $	��0 5q�6J   $	��Top=�6X   $��Stretch=�6p   $
��Auto=�6�   . �5      /�  /� TextAlignment$
 Left=�6#   .T�5      J� .*�6$   .*�5      �� �6"   �� *	Content+5      5      5      �� 5      $��2��6   $��Close��6&   $
��Bold"�66   $��Center=�6H   $��Right=�6c   $	��100��6   $��5q�6�   $��0 10 0 0q�6�   y�5      �� *VisibilityClose+��5      �� *CloseCommand+��5      �� $
 wdNotification���� 5       $ 
Horizontal=�6   $��Right=�66   $��3��6R   .+�5!      �� $	��Yes��6   $
��Bold"�6+   $��Center=�6=   $��Right=�6X   $	��100��6t   $��5q�6�   $��0 10 5 0q�6�   y�5"      �� *VisibilityYesNo+��5#      �� *
YesCommand+��5$      �� $
 wdNotification���� 5%      $��No��6   $
��Bold"�6*   $��Center=�6<   $��Right=�6W   $	��100��6s   $��5q�6   $��5 10 0 0q�6�   y�5&      �� *VisibilityYesNo+��5'      �� *	NoCommand+��5(      �� $
 wdNotification��5)      5*      5+      5,      5-      