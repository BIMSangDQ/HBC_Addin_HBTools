   M S B A M L   `   `   `  ����G  CHoaBinhTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullc6clr-namespace:HoaBinhTools.BIMQAQC.QAQC_Quantity.Views(HoaBinhTools.BIMQAQC.QAQC_Quantity.Views  5   	   :    4HoaBinhTools.BIMQAQC.QAQC_Quantity.Views.QuantityStr   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Hlocal6clr-namespace:HoaBinhTools.BIMQAQC.QAQC_Quantity.Views     5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      -   5          �Name$  wdGenerateModel��5       � Title$ QAQC Quantity Str��5	   	   $	��350��6#   $	��600��60    � 	Resources 5
       �� Source�� 5      $f `pack://application:,,,/HoaBinhTools;component/BIMQAQC/ResourceDictionary/ResourceDictionary.xaml?�6   %    .��5      �� .@�5      �� $
��QAQC��6   $��Italic$�6   .��5      � $��0 5 0 0q�6
    � ColumnDefinitions 5      �� 5      $��7* �6   �� 5      $��3* �6    � RowDefinitions 5      �� 5      $��20 �6   �� 5      $��40 �6   �� 5      �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��30 �6   .+�5       ��  �� Orientation$ 
Horizontal=�6   .+�5&      �� $��Detect overlap��6   $��1��6)      
SDCheckBox#��[  5'       d� 	IsChecked 5(      �� *IsDetectOverlap+�� 5*      $��
Check Void��6   $��1��6%   #��[  5+      $��10 0 0 0q�5-       5,      �� *IsSwitchJoin+5-      5.      �� 50      $��1��6   $��0��6!   $ 
Horizontal=�6.   $��Right=�51      y�52      �� *IsDetectOverlap+.+�54      ��   SDButtonText#��[ 6   $��Time Span : ��55      �� 57      #��[ 6   ��58      �� *
TimeSpan+59      �� 5;      $��0��6   $��1��6!   $ 
Horizontal=�6.   $
��Left=�6G   .+�5<      �� $��32��6   $��32��6   $��Right=�6%   $��1��5=   	   $��1��6     SDButtonDel#��[ 5>      $��0,2,5,2q�6,    � ToolTip$ Isolate All Element��5?      ��5@      �� *btnIsolateAll+.��5A      �� $��24��6   $��24��6   $N��H/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/select-place.png��5B      5C      5D      �� 5F      $��1��6   $��1��6!   $ 
Horizontal=�6.   $��Right=�6G   .+�5G      �� $��32��6   $��32��6   $��Right=�6%   $��1��5H   	   $��1��6   #��[ 5I      $��0,2,5,2q�6,   $ Export��5J      ��5K      �� *	btnExport+.��5L      �� $��24��6   $��24��6   $U��O/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/file-export-outline.png��5M      5N      �� 5P      $��32��6   $��32��6   $��Right=�6%   $��1��5Q   	   $��1��6   #��[ 5R      $��0,2,0,2q�6,   $ import��5S      ��5T      �� *	btnImport+.��5U      �� $��24��6   $��24��6   $U��O/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/file-import-outline.png��5V      5W      5X      &   System.Windows.Controls.DataGrid  5Z      -   5        	  Name$	 dgCheck��5Z      $
��Auto��6      
SDDataGrid#��[ 5[      
   GridLinesVisibility$

 None=�5\         CanUserSortColumns .    CanUserAddRows .  $��2��5_      $��2��6      AutoGenerateColumns .  
������ 
������ $��Hidden=�5e      ��5c      �� *DetectOverlaps+ �� SelectedItem 5d      �� *DetectElement+   Columns 5f      4  .System.Windows.Controls.DataGridTemplateColumn  5g      ,  &System.Windows.Controls.DataGridColumn   MaxWidth$ 25Q�6      CellTemplate 5h   	   �� 5i   
   .R�5j      �� $ 
Horizontal=�6   $
��Left=�6/   .+�5k      �� $��18��6   $��18��6   #��[ 5l      $��2q�64   $ Find Elements��5m      ��5n      ��  �� Path$! DataContext.btnFindElements� �� RelativeSource �� *FindAncestor+ �� AncestorType"  .��5p      �� $��16��6   $��16��6   $I��C/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/magnify.png��5q       5r       �� 
TargetType �� Property �� Value��%^   '��       �� 5s      " ��6   .�5t      ��  �@��CornerRadius$ 5��6.   5u      5v      5w      5x      5y      5{   	   0  *System.Windows.Controls.DataGridTextColumn  5}         Header$ Element��6      Width5  /System.Windows.Controls.DataGridLengthConverter$	 150 5~   ,   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6   �� *Element1CateGory+6,     5�      $ Overlap With��6   $	 150 5�   ,    6   �� *Element2CateGory+6,     5�      $ Overlap Volume (m3)��6   $	 150 5�   )    6   �� *OverlapVolume+6)   5�      � 5�      $��3��6   $��2��6    5�      �� 5�      �� 5�      �� 5�      .+�5�      �� $��0��6   $��3��6"   $ 
Horizontal=�6/   y�5�      �� *IsDetectOverlap+.+�5�      �� #��[ 6   $��Count : ��5�      �� 5�      #��[ 6   ��5�      �� *ElementOverlapCount+5�      �� 5�      $��3��6   $ 
Horizontal=�6"   $��Right=�6;   .+�5�      �� #��[ 6   $��Geometry : ��5�      �� 5�      #��[ 6   ��5�      �� *GeometryVolume+�� 5�      #��[ 6   $	�� m3��5�      5�      5�      � 5�      $��4��6   $��2��6    5�      �� 5�      �� 5�      �� 5�      .+�5�      �� $��3��6   $��3��6"   $ 
Horizontal=�6/   $��Right=�6H   y�5�      �� *IsDetectOverlap+.+�5�      �� #��[ 6   $��
Overlap : ��5�      �� 5�      #��[ 6   ��5�      �� *SumVolumneOverlap+�� 5�      #��[ 6   $	�� m3��5�      5�      5�      � 5�      $��5��6   $��2��6    5�      �� 5�      �� 5�      �� 5�      .+�5�      �� $��2��6   $��3��6"   $ 
Horizontal=�6/   $��Right=�6H   y�5�      �� *IsDetectOverlap+.+�5�      �� #��[ 6   $��Lack of concrete : ��5�      �� 5�      #��[ 6   ��5�      �� *SumLackofConcrete+�� 5�      #��[ 6   $	�� m3��5�      5�      5�      �� 5�      $��6��6   $��2��6   $ 
Horizontal=�62   .+�5�      �� #��[ 6   $��Select ��5�      �� 5�      #��[ 6   ��5�      �� *SelectElementCount+�� 5�      #��[ 6   $��/��5�      �� 5�      #��[ 6   ��5�      �� *ElementCount+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5�      � 5�      $��7��6   $��0��6   $��2��6(   .+�5�      �� $��0��6   $��7��6   $��Check��6+     SDButton#��[ 5�      ��5�      �� *btnCheckModel+5�      5�      5�      �� 5�      $��Setting��6   $��Italic$�6   .��5�      � $��0 5 0 0q�6
    5�      �� 5�      $��25 �6   �� 5�      �� 5�      $��30 �6   .+�5�      �� $	��100��6   $
��Left=�6   $	��5,0q�64   $��Add Exclude Filter��5�      #��[ 5�      ��5�      �� *btnAddExclude+5�      �� 5�      $��1��6   .��5�        -   5        $	 dgFilter��5�      $
��Auto��6"   $��0,5,0,0q�5�      $

 None=�5�       .  .   .  
������ 
������ $��Hidden=�5�      ��5�      �� *ExcludeFilters+$ TwoWay=�$ PropertyChanged=� 5�        5�   	   $ 30Q�6     5�   
   �� 5�      .R�5�      �� $ 
Horizontal=�6   $
��Left=�60   .+�5�      �� $��18��6   $��18��6   #��[ 5      $��2q�64   $ Del this filter��5     ��5     �� $# DataContext.btndgfilterDelete� �� *FindAncestor+"  .��5     �� $��16��6   $��16��6   $^��X/HoaBinhTools;component/BIMQAQC/ModelChecker/Views/Icon/file-document-remove-outline.png��5      5     ��%^   '��       �� 5     " ��6   .�5     ��  �@��CornerRadius$ 5��6/   5	     5
     5     5     5     5  
     5  	      MinWidth$	 150Q�6    $	 150Q�6/   $ Category��6>    5  
   �� 5     .R�5     ��   
SDCombobox#��[ 5     ��5     �� $ 
Categories�$ PropertyChanged=� 5     �� $ CategorySelect�$ PropertyChanged=�5     5     5  
     5  	   $	 150 6    $ Criteria��6,    5  
   �� 5     .R�5     �� $��2q�6   #��[ 5!     ��5     �� $ ListCriteria�$ PropertyChanged=� 5      �� $ Criteria�$ PropertyChanged=�.  (System.Windows.Interactivity.Interaction   Triggers 5"     /  )System.Windows.Interactivity.EventTrigger  5#        	EventName$ SelectionChanged��6   .	  (System.Windows.Interactivity.TriggerBase  	  Actions.  5$     6
  0System.Windows.Interactivity.InvokeCommandAction
  ! 
  Command! 6%   �� $) #DataContext.btnChangeCriteriaFilter� �� *FindAncestor+"  5&     5(     5)     5+  
     5-  	   $	 200 6    $ Property��6,    5.  
   �� 5/     .R�50     ��   	SDTextBox#��[ 6   ��51     �� *
Property+$ PropertyChanged=�{�52     �� *
IsTypeName+53     55  
     57  	   $	 200 6    $ 	Condition��6,    58  
   �� 59     .R�5:     �� $��2q�6   #��[ 5=     ��5;     �� $ ListCondition�$ PropertyChanged=� 5<     �� $ 	Condition�$ PropertyChanged=�5>     5?     5A  
     5C  	   $	 200 6    $ Value��6,    5D  
   �� 5E     .R�5F     �� #��[ 6   ��5G     �� *Value+$ PropertyChanged=�5H     5J  
   5L     5M     �� 5O     $��2��6   $
��Save��5P     #��[ 5R     ��5Q     �� *btnSaveSetting+5R     5T     5U     5X     5Y     