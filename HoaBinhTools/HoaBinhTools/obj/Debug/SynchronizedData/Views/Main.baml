   M S B A M L   `   `   `  ����G  CHoaBinhTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:HoaBinhTools.SynchronizedData.Views#HoaBinhTools.SynchronizedData.Views  5   	   c6clr-namespace:HoaBinhTools.SynchronizedData.Converters(HoaBinhTools.SynchronizedData.Converters  5      .    (HoaBinhTools.SynchronizedData.Views.Main   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      =local1clr-namespace:HoaBinhTools.SynchronizedData.Views  5   	   G
converters6clr-namespace:HoaBinhTools.SynchronizedData.Converters  5        � Title$  Synchronize data��5
   	   $��60��6"   $	��350��5      $	��700��6    � 	Resources 5      @   :HoaBinhTools.SynchronizedData.Converters.OppositeValueBool   OppositeValueBool��%   &	          5      6!   5      .��5      �  � RowDefinitions 5      �� 5      �� 5      $��25 �6    � ColumnDefinitions 5      �� 5      $��50 �6   �� 5      $��* �6   �� 5      $��50 �6   .+�5      �� -   5         ��Name$ TabMain��5       �� TabStripPlacement$	 Top=�5      $��3��5       �� SelectedIndex$ 1��6   .@�5       �� $��	  Group  ��6   $��60��6    .��5!      �  5"      �� 5#      $
��87.5 �6   �� 5$      $��* �6   �� 5%      $��* �6   �� 5&      $��50 �6    5)      �� 5*      $��20 �6   �� 5+      .+�5.      �� $��Group Name:��5/      $��2,2,2,2q�50      �� 51      $��0��52      $��1��6   $��2��6$   $��2,0,2,0q�53      ��54      �� *NewGroupName+�� 55      $��0��56      $��3��6   $��2,0,2,0q�57      $��Create��58      ��59      �� *btnCreateGroup+�� 5;      -   5         ��Name$ ListFile��5;      $��0��5<      $��1��6   $��4��6$   $��2,2,2,2q�5=      ��5>      �� *
FileList+ �� View 5?      �� 5@   	   .P�5A   
   �� 	 �� Width$		 220��6   .��5B      �� $��20��5C      $	��250��6   $��Add File��5D      
������ 
������ ��5G      �� *btnGetFilePath+
 �� CellTemplate
 5I      �� 5J      .R�5K      ��  �� Orientation$ 
Horizontal=�6   $	��250��5L      .+�5M      �� $	��210��5N      $
��Left=�5P      ��5O      �� *	File_Name+5P      5Q      5R      5U      5V   
   5X      5Y      5Z      �� 5\      $��  Synchronized  ��6   $	��100��6'   .��5]      �  5^      �� 5_      $��75 �6   �� 5`      $��75 �6   �� 5a      �� 5b      $	��100 �6    5e      �� 5f      $��25 �6   �� 5g      $��25 �6   �� 5h      .+�5k      �� $��2,2,2,2q�5l      ��5m      �� *
Categories+ �� SelectedItem 5n      �� *CategoryName+ �� Mode$ TwoWay=�.  (System.Windows.Interactivity.Interaction   Triggers 5o      /  )System.Windows.Interactivity.EventTrigger  5p   	      	EventName$ SelectionChanged��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5q   
   6  0System.Windows.Interactivity.InvokeCommandAction     Command 6    �� *btnChangeCategory+5r   
   5t      �� 5v      -   5         ��Name$
 Type��5v      $��2,2,2,2q�5w      $��1��5x      $��2��6    �� 
IsEditable . ��5y      �� *Types+ 5z      �� *
SelectType+$ TwoWay=�68   �� 5|      $ 
Horizontal=�6   $��3��5}   	   .+�5~      �� $��2,2,1,2q�5      $��45��6   $
��Copy��5�      ��5�      �� *btnOk+�� 5�      $��2,2,1,2q�5�      $��45��6   $��Select��5�      ��5�      �� *	btnPick+5�      �� 5�      $��2,2,2,2q�5�      $��1��5�      $��1��6   $��3��6$   $��Download��5�      ��5�      �� *	btnCreate+�� 5�      -   5         ��Name$ 
cbCheckAll��5�      $��	Check All��5�      $��0��5�      $��1��6   $��2,5,2,2q�6$    d� 	IsChecked 5�      �� *
IsCheckAll+ �� UpdateSourceTrigger$ PropertyChanged=� 5�        5�   	   $ Checked��6   . 5�   
      6    �� *btnCheckAll+   CommandParameter 6@   �� 5�   
     5�   	   $ 	Unchecked��6   . 5�   
      6    �� *btnUnCheckAll+ 6B   �� 5�   
   5�      �� 5�      $��0��5�      $��3��6   $��4��6$   $��2,2,2,2q�5�      ��5�      �� *
CenterDb+$ PropertyChanged=�{�5�      ��  �� ElementName$ 
cbCheckAll�� �� Path$ 	IsChecked� �� 	Converter# [   5�      �� 5�   	   .P�5�   
   �� $�� ��6   $	 30��6#   
 5�      �� 5�      .R�5�      ��  6   �� *	IsCheck+$ PropertyChanged=�5�      5�      5�      �� 5�   
   $��Id��6   $	 30��6%   
 5�      �� 5�      .R�5�      �� ��6   �� *Id+5�      5�      �� 5�   
   $��Category��6   $	 60��6+   
 5�      �� 5�      .R�5�      �� ��6   �� *Type+5�      5�      �� 5�   
   $��	Type Name��6   $		 250��6,   
 5�      �� 5�      .R�5�      �� ��6   �� *
TypeName+5�      5�      �� 5�   
   $��	Update By��6   $		 100��6,   
 5�      �� 5�      .R�5�      �� ��6   �� *Author+5�      5�      �� 5�   
   $
��Time��6   $		 130��6'   
 5�      �� 5�      .R�5�      �� ��6   �� *Time+5�      5�      5�   
   5�      5�      5�       5�        5�      $ SelectionChanged��6   . 5�         6   �� *btnTabChange+5�      5�      �� 5�      $��Group:��6   $��2,2,2,2q�5�      $��Center=�5�      $��0��5�      $��1��6   �� 5�      $��1��5�      $��1��6   $��2,2,2,2q�5�       �� DisplayMemberPath$ 
Group_Name��5�      ��5�      �� *
GroupsList+ 5�      �� *CurrentGroup+$ TwoWay=� 5�        5�      $ SelectionChanged��6   . 5�         6   �� *btnChangeGroups+5�      5�      �� 5�      $
��Edit��5�      $��2��5�      $��1��6   $��2,2,2,2q�5�      ��5�      �� *btnEditGroup+5�      5�      