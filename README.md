## Каждая лабораторная — новая ступень
  В этот раз лабораторная научилась работать с бд: ***Northwind***, в моем случае, и даже завелась еще одной службой: ***DataManager*** кличится.

## *DataManager*, или цирк восходящего солнца
  Все нужное берет из ***config***'ов, как и ранее. ***DataManager*** мониторит определенную директорию. Зачем? В ожидании того, что в директории появится файл с указаниями, а точнее: название хранимой процедуры и параметры к ней. Файл может быть как ***.json***, так и ***.xml***. [Примеры](https://github.com/Lirosk/CS2_4/tree/main/Examples/Tasks). Можно, но не нужно, сказать что эти файлы представляют из себя сериализованную сущность класса ***SqlCommand***, действительно не нужно. 
  Почему именно такой подход? 

![hz](https://user-images.githubusercontent.com/62136946/102055956-ae48ed80-3dfc-11eb-97a2-444789a8486e.jpg)

  Я так решил.

## *DataAccess*, или, казалось бы, первые лучи
  С появлением файла на горизонте, в бой вступает ***DataAccess*** с задачей выполнить хранимую процедуру и вернуть результат. При копании в файле из него достаются имя хранимой процедуры и параметры к ней, а еще и название ***Model***'и, ведь никакой статики. [Модели](https://github.com/Lirosk/CS2_4/tree/main/Models). Для примера, в директории появился файл в котором указана хранимая процедура ***OneCityDeals***, а значит ее надо выполнить. [Код процедуры](https://github.com/Lirosk/CS2_4/blob/main/Examples/StoredProcedure/OneCityDeals.sql) и то, что [должна вернуть](https://github.com/Lirosk/CS2_4/blob/main/Examples/StoredProcedure/OneCityDeals.Output.png). 

  Определяем, что от нас хотят.
```C#

var assembly = typeof(Order).Assembly;
var type = assembly.GetType(preSqlCommand.Model);

```
  И возвращаем ***IEnumerable***, ведь к нам вернется таблица, но, т.к. нам же с этим ***IEnumerable*** и работать, прикрепляем к нему тип, что скрывается за ***Generic***-ом.
  
```C#

MethodInfo execute = typeof(SqlCommandExtensions).GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
execute = execute.MakeGenericMethod(type);

var res = new Result<T>()
{
  Table = execute.Invoke(null, new object[] { command }) as IEnumerable<T>,
  TypeOfTable = type
};

return res;
       
```

  Дальнешние купания в лучах, а именно те плясы, которые происходят от рассвета до заката, которые происходят в ***Execute()***, будут происходить [тут](https://github.com/Lirosk/CS2_4/blob/main/DataAccess/Extensions/SqlCommandExtensions.cs).
  
## *ServiceLayer*, или как же быстро гаснет солнце
  Дале требуется наш *IEnumerable* вернуть в *.xml* файле да *.xsd* в аккомпанемент рядом положить, а положить надо не абы куда, а туда, где в ***config***'e указано, в нашем случае, в SourceFolder. Происходит это [здесь](https://github.com/Lirosk/CS2_4/blob/main/ServiceLayer/Layer.cs).
  Дале просиходит то, что происходило на предыдущей ступени: SourceFolder, TargetFolder etc.
  
## В пустоте до рассвета, или "каков звук солнца, озаряющего поляну?" 
  Ожидаем следующий файл.
   
# Итого
  [***ConfigurationManger***](https://github.com/Lirosk/CS2_4/tree/main/ConfigurationManager) — предоставляет нашим службам настройки.
  
  [***DataAccess***](https://github.com/Lirosk/CS2_4/tree/main/DataAccess) — достает данные из бд, исполнив хранимую процедуру.
  
  [***DataManager***](https://github.com/Lirosk/CS2_4/tree/main/DataManager) — мониторит наличие новых указаний, обязует *DataAccess* достать данные, *ServiceLayer* — обернуть да поместить куда надо.
  
  [***FileManger***](https://github.com/Lirosk/CS2_4/tree/main/FileManager) — мониторит посылки от вышестоящего товарища.
  
  [***Models***](https://github.com/Lirosk/CS2_4/tree/main/Models) — содержит модели для представления строк таблиц.
  
  [***Parsers***](https://github.com/Lirosk/CS2_4/tree/main/Parsers) — ну тут все понятно.
  
  [***ServiceLayer***](https://github.com/Lirosk/CS2_4/tree/main/ServiceLayer) — помещает таблицу в *.xml* файл, кладет рядом его *.xsd*.
  
  [***Examples***](https://github.com/Lirosk/CS2_4/tree/main/Examples) — содержит:  
      1) [***Tasks***](https://github.com/Lirosk/CS2_4/tree/main/Examples/Tasks) — примеры передачи указаний(в файле) для выполнения хранимых процедур.     
      2) [***Results***](https://github.com/Lirosk/CS2_4/tree/main/Examples/Results) — результаты выполнения данных процедур.    
      3) [***StoredProcedure***](https://github.com/Lirosk/CS2_4/tree/main/Examples/StoredProcedure) — собственная хранимая процедура, и результаты ее выполнения.
    

  Это конец, спасибо)
