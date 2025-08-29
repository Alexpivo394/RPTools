using QuantityCheck.Dictionaries;
using QuantityCheck.Services;
using System.Collections.Generic;

namespace QuantityCheck.Models;

public class QuantityProcessor
    {
        private readonly Document _doc;
        private readonly Logger? _logger;
        private string? _paramName;

        public QuantityProcessor(Document doc, Logger? logger)
        {
            _logger = logger;
            _doc = doc;
        }

        public void Process(string? paramName)
        {
            _paramName = paramName;
            _logger?.StartLog(string.Empty);
            _logger?.Log($"Старт обработки. Параметр: '{_paramName}'");
            // Собираем все элементы
            var collector = new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType();

            #if REVIT2024_OR_GREATER
            // Штучные
            var pieceElements = collector
                .Where(e => e.Category != null && CategorySets.Sets["Штучные"].Contains((BuiltInCategory)e.Category.Id.Value))
                .ToList();

            // Длинововые
            var linearElements = collector
                .Where(e => e.Category != null && CategorySets.Sets["Длинововые"].Contains((BuiltInCategory)e.Category.Id.Value))
                .ToList();
            #else
                        // Штучные
            var pieceElements = collector
                .Where(e => e.Category != null && CategorySets.Sets["Штучные"].Contains((BuiltInCategory)e.Category.Id.IntegerValue))
                .ToList();

            // Длинововые
            var linearElements = collector
                .Where(e => e.Category != null && CategorySets.Sets["Длинововые"].Contains((BuiltInCategory)e.Category.Id.IntegerValue))
                .ToList();
            #endif
            
            _logger?.Log($"Найдено: штучных={pieceElements.Count}, длинновых={linearElements.Count}");

            using (var tx = new Transaction(_doc, "Заполнение количества"))
            {
                tx.Start();

                var failed = new List<(ElementId id, string reason)>();
                int writtenCount = 0;

                // Штучные
                foreach (var el in pieceElements)
                {
                    var result = WriteQuantity(el, 1);
                    if (result.ok) writtenCount++; else failed.Add((el.Id, result.reason ?? "Неизвестная причина"));
                }

                // Длиновые
                foreach (var el in linearElements)
                {
                    double lengthMm = GetLengthInMm(el);
                    if (lengthMm > 0)
                    {
                        double quantity = (lengthMm / 1000.0) * 1.1;
                        var result = WriteQuantity(el, quantity);
                        if (result.ok) writtenCount++; else failed.Add((el.Id, result.reason ?? "Неизвестная причина"));
                    }
                    else
                    {
                        failed.Add((el.Id, "Не удалось получить длину"));
                    }
                }

                tx.Commit();

                _logger?.Log($"Успешно записано: {writtenCount}");
                if (failed.Count > 0)
                {
                    _logger?.Log($"Не удалось записать: {failed.Count}");
                    foreach (var f in failed)
                    {   
#if REVIT2024_OR_GREATER
                        _logger?.Log($"Не записано. Id={f.id.Value}; причина: {f.reason}");
#else
                        _logger?.Log($"Не записано. Id={f.id.IntegerValue}; причина: {f.reason}");
#endif 
                        
                        
                        
                    }
                }
                else
                {
                    _logger?.Log("Ошибок не обнаружено");
                }
            }
            _logger?.Log("Завершено");
        }

        private (bool ok, string? reason) WriteQuantity(Element el, double value)
        {
            var param = el.LookupParameter(_paramName);
            if (param == null) return (false, "Параметр не найден");
            if (param.IsReadOnly) return (false, "Параметр только для чтения");
            try
            {
                param.Set(value);
                return (true, null);
            }
            catch (Exception ex)
            {
#if REVIT2024_OR_GREATER
                _logger?.LogError($"Ошибка записи параметра. Id={el.Id.Value}", ex);
#else
                _logger?.LogError($"Ошибка записи параметра. Id={el.Id.IntegerValue}", ex);
#endif 
                return (false, "Исключение при записи параметра");
            }
        }

        private double GetLengthInMm(Element el)
        {
            var param = el.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
            if (param != null && param.StorageType == StorageType.Double)
            {
                // длина в футах
                double lengthFeet = param.AsDouble();
                // переводим в мм
                
                #if REVIT2021_OR_GREATER
                double lengthMm = UnitUtils.ConvertFromInternalUnits(lengthFeet, UnitTypeId.Millimeters);
                #else
                double lengthMm = UnitUtils.ConvertFromInternalUnits(lengthFeet, DisplayUnitType.DUT_MILLIMETERS);
                #endif
                return lengthMm;
            }
            return 0;
        }

    }