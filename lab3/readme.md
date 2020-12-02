Программа состоит из двух частей
 *FileWatcherService содержит json, xml и xsd файлы и Windows-службу, которая отвечает за отслеживание состояния SourceD (2лр)
 *ConfigManager содержит IConfigurationParser(интерфейс парсера), OptionsManager(наличие .xml и .json), Provider(выбор конфигурации парсера .xml или .json), JsonParser и XMLParser парсируют свои конфигурации 