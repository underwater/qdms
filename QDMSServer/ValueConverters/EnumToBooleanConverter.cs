using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace QDMSServer
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }

        // Using example 
        /*
         * <Grid>
            <Grid.Resources>
              <l:EnumBooleanConverter x:Key="enumBooleanConverter" />
            </Grid.Resources>
            <StackPanel >
              <RadioButton IsChecked="{Binding Path=VeryLovelyEnum, Converter={StaticResource enumBooleanConverter}, ConverterParameter=FirstSelection}">first selection</RadioButton>
              <RadioButton IsChecked="{Binding Path=VeryLovelyEnum, Converter={StaticResource enumBooleanConverter}, ConverterParameter=TheOtherSelection}">the other selection</RadioButton>
              <RadioButton IsChecked="{Binding Path=VeryLovelyEnum, Converter={StaticResource enumBooleanConverter}, ConverterParameter=YetAnotherOne}">yet another one</RadioButton>
            </StackPanel>
          </Grid>
         */
    }
}
