﻿using HttpServer;
using HttpServer.Sessions;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.SlimTv.Interfaces;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.TAS.Timeshiftings
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "userName", Type = typeof(string), Nullable = false)]
  internal class CancelCurrentTimeShifting
  {
    public WebBoolResult Process(string userName)
    {
      if (!ServiceRegistration.IsRegistered<ITvProvider>())
        throw new BadRequestException("CancelCurrentTimeShifting: ITvProvider not found");

      if (userName == null)
        throw new BadRequestException("CancelCurrentTimeShifting: userName is null");

      ITimeshiftControlEx timeshiftControl = ServiceRegistration.Get<ITvProvider>() as ITimeshiftControlEx;

      bool result = timeshiftControl.StopTimeshift(userName, SlotControl.GetSlotIndex(userName));
      SlotControl.DeleteSlotIndex(userName);


      return new WebBoolResult { Result = result };
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
