   M S B A M L   `   `   `  ����G  CHoaBinhTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullw@clr-namespace:HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar2HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar  5      K    EHoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar.DisciplineCheckSet   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Rlocal@clr-namespace:HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5        N� 
Background
  ����� N� Title$ DisciplineCheckSet��5       � 	Resources 5       �� Source�� 5      $f `pack://application:,,,/HoaBinhTools;component/BIMQAQC/ResourceDictionary/ResourceDictionary.xaml?�6   %    .��5      �  � RowDefinitions 5      �� 5      $��40 �6   �� 5      $��* �6   �� 5      $��5 �6    � ColumnDefinitions 5      �� 5      $	��100 �6   �� 5      �� 5      $��80 �6   .+�5      �� $��0��6   $��0��6   $��	10,15,0,0q�5      $��	Check Set��5      �� 5      $��0��6   $��1��6      
SDCombobox#��[  5       ��5!      �� *DesciplineCheck+ �� SelectedItem 5"      �� *DesciplineCheckName+ �� Mode$ TwoWay=�.  (System.Windows.Interactivity.Interaction   Triggers 5#      /  )System.Windows.Interactivity.EventTrigger  5$      	   	EventName$	 SelectionChanged��6   .  (System.Windows.Interactivity.TriggerBase
   Actions.
 5%      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6   �� *btnCheckSetChange+5&      5(      �� 5*       �� Orientation$ 
Horizontal=�6   $
��Auto��6(   $��0��5+      $��2��6   .+�5-      �� $��32��6   $��32��6     SDButtonDel#��[ 5.      $��0,2,5,2q�6+    � ToolTip$ Duplicate Checkset��5/      ��50      �� *btnDuplicate+.��51      �� $��24��6   $��24��6   $S��M/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/content-duplicate.png��52      53      �� 55      $��32��6   $��32��6   #��[ 56      $��5,2,10,2q�6+   $ Delete Checkset��57      ��58      �� *btnDeleteCheckSet+.��59      �� $��24��6   $��24��6   $^��X/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/file-document-remove-outline.png��5:      5;      5<      � 5>      $��1��6	   $��3��6    5?      �� 5@      $	��300 �6   �� 5A      .+�5D      �� $	��295��6   .��5E      &   System.Windows.Controls.DataGrid  -   5          Name$ dgCheck��5E      $
��Auto��6      
SDDataGrid#��[ 5F   	      GridLinesVisibility$
 None=�5G         CanUserSortColumns .    CanUserAddRows .     AutoGenerateColumns .  
������ 
������ $��Hidden=�5O      ��5M      �� *DesciplineCheckRows+ 5N      �� *DisciplineCheckRow+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�   Columns 5P      4  .System.Windows.Controls.DataGridTemplateColumn  5R      ,  &System.Windows.Controls.DataGridColumn   MaxWidth$ 25Q�6      CellTemplate 5S   	   �� 5T   
   .R�5U      �� $ 
Horizontal=�6   $
��Left=�6/   .+�5V      �� $��18��6   $��18��6   #��[ 5W      $��2q�64   $ 	Move down��5X      ��5Y      ��  �� Path$$ DataContext.btndgCheckMoveDown� �� RelativeSource �� *FindAncestor+ �� AncestorType"  ��5[      �� $
 Name�.��5\      �� $��16��6   $��16��6   $Y��S/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/arrow-down-bold-outline.png��5]       5^       �� 
TargetType �� Property �� Value��%^   '��       �� 5_      " ��6   .�5`      ��  �@��CornerRadius$ 5��6.   5a      5b      5c      5d      5e      5g   	     5i      $ 25Q�6    5j   	   �� 5k   
   .R�5l      �� $ 
Horizontal=�6   $
��Left=�6/   .+�5m      �� $��18��6   $��18��6   #��[ 5n      $��2q�64   $ Move up��5o      ��5p      �� $" DataContext.btndgCheckMoveUp� �� *FindAncestor+"  ��5r      �� $
 Name�.��5s      �� $��16��6   $��16��6   $W��Q/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/arrow-up-bold-outline.png��5t       5u      ��%^   '��       �� 5v      " ��6   .�5w      ��  �@��CornerRadius$ 5��6.   5x      5y      5z      5{      5|      5~   	     5�         Header$ 
List check��6      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 63    5�   	   �� 5�   
   .R�5�      ��   SDButtonDel2#��[ 6   $
��Left=�5�      ��5�      �� $
 Name� 5�      �� $
 Name���5�      �� $  DataContext.btnSelectCheck� �� *FindAncestor+"  ��5�      �� $
 Name�5�      5�      5�   	   5�      5�      � 5�      $��1��6
    5�      �� 5�      $
��Auto �6   �� 5�      $
��Auto �6   �� 5�      $
��Auto �6   �� 5�      $
��Auto �6   �� 5�      $
��Auto �6   �� 5�      $��40 �6   �� 5�      $��40 �6   �� 5�       5�      �� 5�      $	��120 �6   �� 5�      .+�5�      �� $��Name:��6     SDButtonText#��[ 6   $��0��5�   
   $��	20,15,0,5q�6   � 5�      $��0��6   $��1��6   $��Stretch=�6(   $��0q�6F    5�      �� 5�      �� 5�      $��40 �6   �� 5�      $��40 �6   �� 5�      $��40 �6   .+�5�      ��   	SDTextBox#��[ 6   $
��Auto��5�   	    �� TextWrapping$
 Wrap=�5�      ��5�      �� *	CheckName+5�      �� 5�      $��32��6   $��32��6   $��Right=�6%   $��1��5�      #��[ 5�      $��0,2,5,2q�6+   $ Duplicate this check��5�      ��5�      �� *btnAddCheck+.��5�      �� $��24��6   $��24��6   $S��M/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/content-duplicate.png��5�      5�      �� 5�      $��32��6   $��32��6   $��Right=�6%   $��2��5�      #��[ 5�      $��0,2,5,2q�6+   $ Save this Check��5�      ��5�      �� *btnSaveCheck+.��5�      �� $��24��6   $��24��6   $\��V/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/content-save-check-outline.png��5�      5�      �� 5�      $��32��6   $��32��6   $��Right=�6%   $��3��5�      #��[ 5�      $��0,2,5,2q�6+   $ Delete this check��5�      ��5�      �� *btnDeleteCheck+.��5�      �� $��24��6   $��24��6   $^��X/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/file-document-remove-outline.png��5�      5�      5�      �� 5�      $��Description:��6   #��[ 6$   $��1��5�   
   $��	20,15,0,5q�6   �� 5�      #��[ 6   $��1��5�      $��1��6   ��5�      �� *Description+5�      �� 5�      $��Check Result:��6   #��[ 6%   $��2��5�   
   $��	20,15,0,5q�6   �� 5�      #��[  6   $��2��5�      $��1��6   ��5�   	   �� *Check_Results+ 5�   	   �� *Check_Result+5�      �� 5�      $��Failure Message:��6   #��[ 6(   $��3��5�   
   $��	20,15,0,5q�6   �� 5�      #��[ 6   $��3��5�      $��1��6   ��5�      �� *Failure_Message+5�      �� 5�      $��Ratings:��6   #��[ 6    $��4��5�   
   $��	20,15,0,5q�6   � 5�      $��4��6   $��1��6    5�      �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $��35 �6   �� 5�      $	��120 �6   �� 5�      .+�5�      ��   SDTextBoxNoBackgroud2#��[ 5      $��0��5     $��0��5     {�.  5     �� 5     
��� �|�
��� �|�#��[ 5     $��1��5     �� 5
     -   5          ��Name$  tbRangeA��5
     #��[ 5     $��2��5     ��5     �� *RangeA+.��5      5       5  	   $	 LostKeyboardFocus��6   .
 5  
      6    �� *btnCheckRangeA+!   CommandParameter! 5     �� " �� ElementName$" tbRangeA��$
 Text�5  
   .��5      5     �� 5     
��� ���
��� ���#��[ 5     $��3��5     �� 5     -   5        $  tbRangeB��5     #��[ 5     $��4��5     ��5     �� *RangeB+.��5      5       5   	   $	 LostKeyboardFocus��6   .
 5!  
      6    �� *btnCheckRangeA+! 5"     �� $" tbRangeB��$
 Text�5#  
   .��5$      5%     �� 5'     
���  ��
���  ��#��[ 5)  	   $��5��5*  	   �� 5,     -   5        $  tbRangeC��5,     #��[ 5-     $��6��5/     ��5.     �� *RangeC+.��5/      50       51  	   $	 LostKeyboardFocus��6   .
 52  
      6    �� *btnCheckRangeA+! 53     �� $" tbRangeC��$
 Text�54  
   .��55      56     �� 58     
���� ��
���� ��#��[ 5:  	   $��7��5;  	   �� 5=     $��Parameters Result:��6   #��[ 6+   $��8��5>  
   $��	20,15,0,5q�6   �� 5@     #��[ 6   $��9��5B     ��5A     �� *ParasInResult+5B     5C     �� 5E     $��5��6   $��	20,10,5,5q�6   $��Set this check to run��5G  	     
SDCheckBox#��[ 6)   # d� 	IsChecked# 5F  	   �� *Run_ThisCheck+5G  )   �� 5I     $��5��6   $��	20,10,5,5q�6   $��1��6/   $��2��6?   $��Right=�5J  	   $��Is Mandatory Condition��5L  	   #��[ 6*   # 5K  	   �� *
IsImpotant+5L  *   �� 5N     $��70��6   $��6��6   $��
Add Filter��6%   $
��Left=�6:   $��0,0,0,10q�5O       SDButton#��[ 5Q     ��5P     �� *btnAddFilter+5Q     �� 5S     $��7��6   $��2��6    .��5T       -   5        $ dgFilter��5T     $
��Auto��6"   $��0,5,0,0q�5U     $
 None=�5V      .  .   .  
������ 
������ $��Hidden=�5]     ��5\     �� *DesciplineCheckRowFilters+$ TwoWay=�$ PropertyChanged=� 5^       5`  	   $ 30Q�6     5a  
   �� 5b     .R�5c     �� $ 
Horizontal=�6   $
��Left=�60   .+�5d     �� $��18��6   $��18��6   #��[ 5e     $��2q�64   $ Del this filter��5f     ��5g     �� $# DataContext.btndgfilterDelete� �� *FindAncestor+"  .��5i     �� $��16��6   $��16��6   $^��X/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/file-document-remove-outline.png��5j      5k     ��%^   '��       �� 5l     " ��6   .�5m     ��  �@��CornerRadius$ 5��6/   5n     5o     5p     5q     5r     5t  
     5v  	   $ 20Q�6     5w  
   �� 5x     .R�5y     �� $ 
Horizontal=�6   $
��Left=�60   .+�5z     �� $��18��6   $��18��6   #��[ 5{     $��2q�64   $ 	Move down��5|     ��5}     �� $% DataContext.btndgfilterMoveDown� �� *FindAncestor+"  .��5     �� $��16��6   $��16��6   $Y��S/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/arrow-down-bold-outline.png��5�      5�     ��%^   '��       �� 5�     " ��6   .�5�     ��  �@��CornerRadius$ 5��6/   5�     5�     5�     5�     5�     5�  
     5�  	   $ 20Q�6     5�  
   �� 5�     .R�5�     �� $ 
Horizontal=�6   $
��Left=�60   .+�5�     �� $��18��6   $��18��6   #��[ 5�     $��2q�64   $ Move up��5�     ��5�     �� $# DataContext.btndgfilterMoveUp� �� *FindAncestor+"  .��5�     �� $��16��6   $��16��6   $W��Q/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/arrow-up-bold-outline.png��5�      5�     ��%^   '��       �� 5�     " ��6   .�5�     ��  �@��CornerRadius$ 5��6/   5�     5�     5�     5�     5�     5�  
     5�  	   $   MinWidth$	$ 150Q�6    $	 150Q�6/   $ Oparator��6>    5�  
   �� 5�     .R�5�     �� $��80��6   #��[  5�     ��5�     �� $ 	Oparators�$ PropertyChanged=� 5�     �� $ Oparator�$ PropertyChanged=�5�     5�     5�  
     5�  	   $	 150 6    $ Criteria��6,    5�  
   �� 5�     .R�5�     �� $��2q�6   #��[  5�     ��5�     �� $ 	Criterias�$ PropertyChanged=� 5�     �� $ Criteria�$ PropertyChanged=� 5�       5�     $	 SelectionChanged��6   .
 5�        6%   �� $) #DataContext.btnChangeCriteriaFilter� �� *FindAncestor+"  5�     5�     5�     5�  
     5�  	   $	 200 6    $ Property��6,    5�  
   �� 5�     .R�5�     �� $��2q�6    	 SDComboboxIsEdit#��[	 5�     ��5�     �� $ 
Properties�$ PropertyChanged=� 5�     �� $ Property�$ PropertyChanged=� 5�       5�     $	 SelectionChanged��6   .
 5�        6%   �� $# DataContext.btnChangeProperty� �� *FindAncestor+"  5�       5�     $	 KeyUp��6   .
 5�        6%   �� $# DataContext.btnFilterProperty� �� *FindAncestor+"  5�     5�     5�     5�  
     5�  	   $	 200 6    $ 	Condition��6,    5�  
   �� 5�     .R�5�     �� $��2q�6   #��[  5�     ��5�     �� $ 
Conditions�$ PropertyChanged=� 5�     �� $ 	Condition�$ PropertyChanged=� 5�       5�     $	 SelectionChanged��6   .
 5�        6%   �� $% DataContext.btnChangeConditions� �� *FindAncestor+"  5�     5�     5�     5�  
     5�  	   $	 200 6    $ Value��6,    5�  
   �� 5�     .R�5�     �� #��[ 6   ��5�     �� *Value+$ PropertyChanged=�5�     5�  
   5�     5�     5�     5�     5�     5�     