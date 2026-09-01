using BuildingBlocks.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Schedules.Application.Schedules.DeleteSchedule;

public sealed record DeleteScheduleCommand(
    Guid Id
) : IRequest<Result>;