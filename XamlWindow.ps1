[xml]$XAMLMain = @'
<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Demo application" Height="350" Width="525">
    	<Grid>
        	<Label x:Name="label" Content="PowerShell + XAML demo application." HorizontalAlignment="Center" Margin="38,10,37.4,0" VerticalAlignment="Top" RenderTransformOrigin="1.056,1.635" Cursor="" FontSize="24" FontWeight="Bold"/>
        	<Border BorderBrush="Black" BorderThickness="1" Height="238" Margin="20,57,20,0" VerticalAlignment="Top"/>
<DataGrid AlternatingRowBackground="LightBlue">
<DataGrid.Columns>
	<DataGridTextColumn Header="Variable"  Binding="{Binding Path=Name}"/>
	<DataGridCheckBoxColumn Header="Is Secret"  Binding="{Binding Path=IsSecret}"/>
	
</DataGrid.Columns>
</DataGrid>
	    </Grid>
</Window>
'@