
1 - Left implementation of IHistoricalData for time being to allow project to compile
2 - Added Task based GetDataAsync implementation using the HTTPClient 
3 - EOD uses WCF Service, so I've added service reference and configuration section to App.Config
4 - Since EOD uses its own wrapper for prices quotes (called QUOTE), I've added seperate parser in EODUtils class

Todo
1- Added Exception Handling
2 - Need to ask if IHistoricalDataSource will be converted from event-based asynchronous model to Task-based Asynchronous model ? 
3 - If not, need to see how to convert Task based results back to Events

