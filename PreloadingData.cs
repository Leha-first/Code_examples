using System.Linq;
using System.Threading;
using CommonExtensions;
using ASCRV.Base.ASCRV_;
using ASCRV.CommonClasses;
using System.Threading.Tasks;


namespace ASCRV.SearchByRequest
{
    using System;
    using System.Collections.Generic;

    public partial class MainController
    {
        /// <summary>
        /// Метод выполняемый при старте программы
        /// </summary>
        private void PreloadingData()
        {
            mainModel.CanCloseModule = false;
            mainModel.ProcessInformation = "В данный момент происходит добавление элементов коллекций. Не рекомендуется закрывать модуль.";
            mainViewAdapter.DaisyViewStartTip("Идет предварительная загрузка данных");
            var currentException = new List<Exception>();
            var listWithFormsExistingInBase = new List<Form>();
            var dictionaryWithFormPeriod = new List<GetPeriodsForm>();
            //ПОЛУЧЕНИЕ ФОРМ
            var firstTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        //ПОЛУЧЕНИЕ ВСЕХ ФОРМ
                        var formsSelectException = client.GetAllForms(out var outDataForms);
                        if (formsSelectException != null) throw new Exception(formsSelectException.Message);
                        //ПОЛУЧЕНИЕ СУЩЕСТВУЮЩИХ ФОРМ
                        var exceptionDistinctForms = client.GetDistinctFormsFromDan(out var listWithForms);
                        if (exceptionDistinctForms != null) throw new Exception(exceptionDistinctForms.Message);
                        //ПОЛУЧЕНИЕ ФОРМ ПО ИДЕНТИФИКАТОРУ
                        listWithFormsExistingInBase = listWithForms
                            .Select(formId => outDataForms.LastOrDefault(s => s.ID_Form == formId))
                            .Where(neededForm => neededForm != null).ToList();

                        retList.Add(null);
                        retList.Add(outDataForms);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null)
                        currentException.Add((Exception)sRes.Result[0]);
                    if (sRes.Result[1] == null) return;
                    foreach (var f in (List<Form>)sRes.Result[1])
                        mainModel.ListOfAllForms.Add(f);
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            //ПОЛУЧЕНИЕ ТАБЛИЦ
            var secondTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        //ПОЛУЧЕНИЕ ВСЕХ ТАБЛИЦ
                        var selectTabsException = client.Select_All_Tabs(out var outDataTabs);
                        if (selectTabsException != null) throw new Exception(selectTabsException.Message);

                        retList.Add(null);
                        retList.Add(outDataTabs);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
            {
                if (sRes.Result[0] != null)
                    currentException.Add((Exception)sRes.Result[0]);
                if (sRes.Result[1] == null) return;
                foreach (var t in (List<ClassSelectTabs>)sRes.Result[1])
                    mainModel.ListWithAllTables.Add(t);
            },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            //ПОЛУЧЕНИЕ ОПИСАНИЯ ПОКАЗАТЕЛЕЙ ТАБЛИЦ
            var thirdTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        //ПОЛУЧЕНИЕ ОПИСАНИЯ ПОКАЗАТЕЛЕЙ ТАБЛИЦ
                        var exceptionDefTab = client.Select_All_DefTab(out var outDataDefTab);
                        if (exceptionDefTab != null) throw new Exception(exceptionDefTab.Message);

                        retList.Add(null);
                        retList.Add(outDataDefTab);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null)
                        currentException.Add((Exception)sRes.Result[0]);
                    if (sRes.Result[1] == null) return;
                    foreach (var d in (List<ClassSelectDefTab>)sRes.Result[1])
                        mainModel.ListWithAllDefTab.Add(d);
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            //ПОЛУЧЕНИЕ ТЕРМИНОВ
            var fourthTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        //ПОЛУЧЕНИЕ ТЕРМИНОВ
                        var exceptionSelectTerms = client.GetTerms(out var outDataTerms);
                        if (exceptionSelectTerms != null) throw new Exception(exceptionSelectTerms.Message);

                        retList.Add(null);
                        retList.Add(outDataTerms);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null)
                        currentException.Add((Exception)sRes.Result[0]);
                    if (sRes.Result[1] == null) return;
                    foreach (var t in (List<ClassTerm>)sRes.Result[1])
                        mainModel.ListWithAllTerms.Add(t);
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            //ПОЛУЧЕНИЕ ВСЕХ ПЕРИОДОВ НА ТЕКУЩИЙ ГОД
            var fifthTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    var listYears = new List<string>();
                    using (var client = mainModel.SetAscrvDataClient()) { listYears = client.GetYearsList(); }
                    
                    var listWithYears = new List<string>();
                    var isSuccessParseMinYear = int.TryParse(listYears.Min(), out var minYear);
                    if (!isSuccessParseMinYear) minYear = 2000;
                    var isSuccessParseMaxYear = int.TryParse(listYears.Max(), out var maxYear);
                    if (!isSuccessParseMaxYear) maxYear = DateTime.Now.Year;

                    for (int year = minYear; year <= maxYear; year++)
                        listWithYears.Add(year.ToString());

                    var periodList = new List<CommonClasses.GetPeriodsClasses.Period>();
                    var listTasks = new List<System.Threading.Tasks.Task>();
                    var lockObj = new object();

                    //ПОЛУЧЕНИЕ ПЕРИОДОВ
                    foreach (var year in listWithYears)
                        listTasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in CommonClasses.GetPeriodsClasses.ClassGetPeriods.GetPeriodsReturnAllPeriods(int.Parse(year)))
                            {
                                item.Year = year;
                                lock (lockObj) { periodList.Add(item); }
                            }
                        }));

                    listTasks.ForEach(s => s.Wait());

                    retList.Add(null);
                    retList.Add(listYears);
                    retList.Add(periodList);
                    return retList;
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null)
                        currentException.Add((Exception) sRes.Result[0]);
                    if (sRes.Result[1] == null) return;


                    mainModel.MinYearDan = ((List<string>)sRes.Result[1]).Min();
                    mainModel.MaxYearDan = ((List<string>)sRes.Result[1]).Max();

                    foreach (var p in (List<CommonClasses.GetPeriodsClasses.Period>) sRes.Result[2])
                        mainModel.ListWithCumulativeSearchingPeriods.Add(p);
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            

            //ПОЛУЧЕНИЕ ПЕРИОДОВ
            var sixthTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        var periodsOfForms = client.Get_Periods_Forms();
                        periodsOfForms.ForEach(s => s.ID_Period = s.ID_Period.Substring(0, 1));
                        //ФОРМИРОВАНИЕ ПО ИДЕНТИФИКАТОРАМ ЭКЗЕМПЛЯРОВ РАССМАТРИВАЕМЫХ ПЕРИОДОВ
                        dictionaryWithFormPeriod.AddRange(periodsOfForms.Select(s => new GetPeriodsForm {ID_Form = s.ID_Form, ID_Period = s.ID_Period} )
                            .Distinct().ToList());
                        retList.Add(null);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null)
                        currentException.Add((Exception)sRes.Result[0]);

                    mainModel.CollectionOfPeriodsForms.Insert(0,
                        new SearchingPeriod { NameOfPeriod = "Все периоды" });
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());


            //ПОЛУЧЕНИЕ ЗОН НАДЗОРА, ТИПОВ И ЛИНКОВ
            var seventhTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        var fullZonesException = client.SelectSurveillanceZonesByNumGsn(" ГЦ00000", out var outDataZones, out var outDataLinks, out var outDataTypes);
                        if (fullZonesException != null) throw new Exception(fullZonesException.Message);

                        var linksElements = outDataLinks.Where(s => s.SurveillanceZonesLinkZoneId == mainModel.CurrentUser.SystemUserCgsn).ToList();
                        linksElements.ForEach(s => s.SurveillanceZonesLinkParentZoneId = null);

                        retList.Add(null);
                        retList.Add(outDataZones);
                        retList.Add(outDataTypes);
                        retList.Add(outDataLinks);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null)
                        currentException.Add((Exception) sRes.Result[0]);
                    if (sRes.Result[1] == null) return;

                    foreach (var s in (List<ClassNewSurveillanceZones>) sRes.Result[1])
                        mainModel.ListCommonControlsClassSurveillanceZones.Add(
                            new CommonControls.SurveillanceZones.Classes.ClassSurveillanceZones
                            {
                                SurveillanceZoneId = s.SurveillanceZoneId,
                                Accessable = string.IsNullOrEmpty(s.SurveillanceZoneDateRemoved?.ToString()) ? 1 : 0,
                                SurveillanceZoneName = s.SurveillanceZoneName,
                                SurveillanceZoneSubTypeId = s.SurveillanceZoneSubTypeId,
                                SurveillanceZoneTypeId = s.SurveillanceZoneTypeId,
                                Removed = s.SurveillanceZoneDateRemoved?.ToString()
                            });

                    foreach (var s in (List<ClassSurveillanceZonesTypes>)sRes.Result[2])
                        mainModel.ListCommonControlsClassSurveillanceZonesTypes.Add(
                            new CommonControls.SurveillanceZones.Classes.ClassSurveillanceZonesTypes
                            {
                                SurveillanceZonesSubTypeId = s.SurveillanceZonesSubTypeId,
                                SurveillanceZonesTypeId = s.SurveillanceZonesTypeId,
                                SurveillanceZonesTypeName = s.SurveillanceZonesTypeName,
                                SurveillanceZonesTypeExplanation = s.SurveillanceZonesTypeExplanation
                            });

                    foreach (var s in (List<ClassSurveillanceZonesLinks>)sRes.Result[3])
                        mainModel.ListCommonControlsClassSurveillanceZonesLinks.Add(
                            new CommonControls.SurveillanceZones.Classes.ClassSurveillanceZonesLinks
                            {
                                SurveillanceZonesLinkId = s.SurveillanceZonesLinkId,
                                SurveillanceZonesLinkParentZoneId = s.SurveillanceZonesLinkParentZoneId,
                                SurveillanceZonesLinkZoneId = s.SurveillanceZonesLinkZoneId
                            });
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
            
            //ПОЛУЧЕНИЕ ФИКСИРОВАННЫХ ЗАПРОСОВ
            var ninethTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                seventhTask.Wait();
                List<object> retList = new List<object>();
                try
                {
                    using (var client = mainModel.SetAscrvDataClient())
                    {
                        //ПОЛУЧЕНИЕ ОБЩИХ И ПЕРСОНАЛЬНЫХ ЗАПРОСОВ
                        var selectFixedQueries = client.SelectCommonAndPersonalFixedQueriesByUserId(mainModel.CurrentUser.SystemUserId, out var outDataFixedQueries);
                        if (selectFixedQueries != null) throw new Exception(selectFixedQueries.Message);
                        retList.Add(null);
                        retList.Add(outDataFixedQueries);
                        return retList;
                    }
                }
                catch (Exception ex)
                {
                    retList.Add(ex);
                    retList.Add(null);
                    return retList;
                }
            }).ContinueWith(sRes =>
                {
                    if (sRes.Result[0] != null) currentException.Add((Exception)sRes.Result[0]);
                    if (sRes.Result[1] == null) return;
                    foreach (var commonQuery in ((List<ClassFixedQueries>) sRes.Result[1]).Where(s => s.FixedQueryType == "C"))// ОБЩИЕ, ДОСТУПНЫЕ ДЛЯ ВСЕХ ЗАПРОСЫ
                        mainModel.CollectionWithCommonFixedQueries.Add(commonQuery);
                    foreach (var personalQuery in ((List<ClassFixedQueries>)sRes.Result[1]).Where(s => s.FixedQueryType == "P"))// ПЕРСОНАЛЬНЫЕ ЗАПРОСЫ
                        mainModel.CollectionWithPersonalFixedQueries.Add(personalQuery);
                    //ДОБАВЛЕНЕ СТАНДАРТНЫХ АТРИБУТОВ
                    //AddStandartAttributes();
                    mainViewAdapter.DaisyViewStop();
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            //ОЖИДАНИЕ ЗАВЕРШЕНИЯ ВСЕХ ЗАДАЧ
            System.Threading.Tasks.Task.Factory.ContinueWhenAll(new[] { firstTask, secondTask, thirdTask, fourthTask, fifthTask , sixthTask, seventhTask, ninethTask },
                taskResults =>
                {

                }).ContinueWith(sRes =>
            {
                if (currentException.Count > 0)
                {
                    ASCRV.CommonDataModel.CommonDataModelInteractions.ExceptionOccured(mainModel.ModuleId, currentException[0]);
                    mainViewAdapter.DaisyViewStartException("", currentException[0].Message, new List<string>());
                    mainModel.CanCloseModule = true;
                    return;
                }

                FillCollectionOfForms(listWithFormsExistingInBase);

                var listWitAllPeriodByForm = new List<SearchingPeriod>();
                foreach (var form in mainModel.CollectionOfForms)
                {
                    var periodsByFormId = dictionaryWithFormPeriod.Where(s => s.ID_Form == form.ID_Form).ToList();
                    periodsByFormId.ForEach(s => listWitAllPeriodByForm.Add(new SearchingPeriod
                        {Period = s.ID_Period, NameOfPeriod = s.ID_Period.TransformTypePeriodToNamePeriod()}));
                }

                listWitAllPeriodByForm.DistinctBy(s => s.Period).ToList().ForEach(s => mainModel.CollectionOfPeriodsForms.Add(s));

                mainViewAdapter.BuildMapControl();

                FormingTheLocalAlphabet();

                mainModel.CanCloseModule = true;
            },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
        /// <summary>
        /// Метод предназначен для добавления стандартных аттрибутов
        /// </summary>
        public void AddStandartAttributes(int countAttributesInBokovik)
        {
            for (int i = 1; i < countAttributesInBokovik; i++)
                mainModel.ListWithAttributes.Add(new ControlListViewAttribute
                {
                    PropertyBokovikNames = new List<ClassBokovikNames>(),
                    PropertyHeader = string.Empty,
                    PropertyIsBokovik = true,
                    PropertyIsEditable = false,
                    PropertyName = i.ToString(),
                    PropertyTypeDan = DefTabTypes.Text,
                    PropertyWidthInPercent = i == 1 ? 15 : 5//ДЛЯ СТРОКИ УСТАНОВИМ БОЛЬШУЮ ШИРИНУ
                });
        }

        /// <summary>
        /// Метод заполнения коллекции с формами с учетом форм - списков( не подгружаются)
        /// </summary>
        /// <param name="listWithFormsExistingInBase"> Список с формами существующими в базе данных </param>
        private void FillCollectionOfForms(List<Form> listWithFormsExistingInBase)
        {
            //получим список форм-списков, по ним нельзя получить строки, соответсвенно для этой задачи они не актуальны
            var collectionOfStaticTables = mainModel.ListWithAllTables.Where(s => s.Static == 2).ToList();

            //отсортированные формы по году добавляем в выходную коллекцию
            foreach (var form in listWithFormsExistingInBase.OrderBy(s => s.ID_Form))
            {
                var isExistElements =
                    collectionOfStaticTables.Where(s => s.ID_Form == form.ID_Form).ToList();
                //если по этой форме не нашлись таблицы с параметром Static=2, то добавляем их в коллекцию
                if (isExistElements.Count != mainModel.ListWithAllTables
                        .Where(s => s.ID_Form == form.ID_Form && s.F_Year == form.Year).ToList()
                        .Count)
                {
                    mainModel.ListOfFormsAfterPreloadingData.Add(form);
                    mainModel.CollectionOfForms.Add(form);
                }
            }
        }

        /// <summary>
        /// Метод для заполнения алфавита для условий
        /// </summary>
        private void FormingTheLocalAlphabet()
        {
            var resultListAlphabet = new List<string>
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z"
            };

            foreach (var itemAlphabet in resultListAlphabet)
                mainModel.LocalAlphabetConditions.Add(itemAlphabet);
        }
    }
}
