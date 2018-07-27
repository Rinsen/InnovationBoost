using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.ExternalApplications;
using Rinsen.InnovationBoost.Models;
using Rinsen.Logger;
using Rinsen.Logger.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Controllers
{
    [Authorize("AdminsOnly")]
    public class LoggerController : Controller
    {
        private readonly ILogReader _logReader;
        private readonly ILogWriter _logWriter;
        private readonly LogHandler _logHandler;
        private readonly ISettingsManager _settingsManager;
        private readonly IExternalApplicationStorage _externalApplicationStorage;

        public LoggerController(ILogReader logReader,
            ILogWriter logWriter,
            LogHandler logHandler,
            IExternalApplicationStorage externalApplicationStorage,
            ISettingsManager settingsManager)
        {
            _logReader = logReader;
            _logWriter = logWriter;
            _logHandler = logHandler;
            _externalApplicationStorage = externalApplicationStorage;
            _settingsManager = settingsManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public Task<bool> Report([FromBody]LogReport logReport)
        {
            return _logHandler.CreateLogs(logReport);
        }

        public async Task<IActionResult> Index()
        {
            var model = new LoggerModel
            {
                SelectionOptions = new SelectionOptions
                {
                    LogApplications = (await GetLogApplications()).ToArray(),
                    LogEnvironments = (await GetLogEnvironments()).ToArray(),
                    LogSources = (await GetLogSources()).ToArray(),
                    LogLevels = GetLogLevels(),
                    From = DateTimeOffset.Now.AddHours(-24),
                    To = DateTimeOffset.Now.AddHours(5)
                }
            };

            await ApplySavedSelection(model);

            return View(model);
        }

        private async Task<IEnumerable<SelectionLogSource>> GetLogSources()
        {
            return (await _logReader.GetLogSourcesAsync()).Select(ls => new SelectionLogSource { Id = ls.Id, Name = ls.Name });
        }

        [HttpPost]
        public async Task<IEnumerable<LogResult>> GetLogs([FromBody]SearchModel searchModel)
        {
            await UpdateSelectionModel(searchModel);

            var logViews = await _logReader.GetLogsAsync(searchModel.From, searchModel.To, searchModel.LogApplications, searchModel.LogEnvironments, searchModel.LogSources, searchModel.LogLevels);

            return logViews.Select(log => new LogResult(log));
        }

        private async Task<IEnumerable<SelectionLogEnvironment>> GetLogEnvironments()
        {
            return (await _logReader.GetLogEnvironmentsAsync()).Select(le =>
            {
                return new SelectionLogEnvironment { Id = le.Id, Name = le.Name };
            });
        }

        private async Task<IEnumerable<SelectionLogApplication>> GetLogApplications()
        {
            return (await _externalApplicationStorage.GetAllAsync()).Select(la =>
            {
                return new SelectionLogApplication { Id = la.Id, Name = la.Name };
            });
        }

        private IEnumerable<SelectionLogLevel> GetLogLevels()
        {
            return new List<SelectionLogLevel>
            {
                new SelectionLogLevel { Level = 0, Name = "Trace"},
                new SelectionLogLevel { Level = 1, Name = "Debug"},
                new SelectionLogLevel { Level = 2, Name = "Information"},
                new SelectionLogLevel { Level = 3, Name = "Warning" },
                new SelectionLogLevel { Level = 4, Name = "Error" },
                new SelectionLogLevel { Level = 5, Name = "Critical" },
            };
        }

        private Task UpdateSelectionModel(SearchModel searchModel)
        {
            var selectionModel = new SelectionModel
            {
                LogApplications = searchModel.LogApplications,
                LogEnvironments = searchModel.LogEnvironments,
                LogLevels = searchModel.LogLevels,
                LogSources = searchModel.LogSources
            };

            return _settingsManager.Set("LoggerSelectionModel", User.GetClaimGuidValue(ClaimTypes.NameIdentifier), selectionModel);
        }

        private Task<SelectionModel> GetSelectionModel()
        {
            return _settingsManager.GetValueOrDefault<SelectionModel>("LoggerSelectionModel", User.GetClaimGuidValue(ClaimTypes.NameIdentifier));
        }

        private async Task ApplySavedSelection(LoggerModel model)
        {
            var selectionModel = await GetSelectionModel();

            if (selectionModel == default(SelectionModel)) // Bail from this if no presaved setting is available
            {
                return;
            }

            foreach (var logApplication in model.SelectionOptions.LogApplications)
            {
                if (selectionModel.LogApplications.Contains(logApplication.Id))
                {
                    logApplication.Selected = true;
                }
            }

            foreach (var logEnvironments in model.SelectionOptions.LogEnvironments)
            {
                if (selectionModel.LogEnvironments.Contains(logEnvironments.Id))
                {
                    logEnvironments.Selected = true;
                }
            }

            foreach (var logLevel in model.SelectionOptions.LogLevels)
            {
                if (selectionModel.LogLevels.Contains(logLevel.Level))
                {
                    logLevel.Selected = true;
                }
            }

            foreach (var logSource in model.SelectionOptions.LogSources)
            {
                if (selectionModel.LogSources.Contains(logSource.Id))
                {
                    logSource.Selected = true;
                }
            }
        }

        private async Task<List<LogEnvironment>> GetLogEnvironments(LogReport logReport)
        {
            var environmentNames = logReport.LogItems.Select(m => m.EnvironmentName).Distinct();

            var logEnvironments = await _logReader.GetLogEnvironmentsAsync();

            foreach (var environmentName in environmentNames)
            {
                if (!logEnvironments.Any(m => m.Name == environmentName))
                {
                    logEnvironments.Add(await _logWriter.CreateLogEnvironmentAsync(environmentName));
                }
            }

            return logEnvironments;
        }

    }
}
