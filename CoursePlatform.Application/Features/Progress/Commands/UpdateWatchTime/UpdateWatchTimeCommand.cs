using MediatR;

namespace CoursePlatform.Application.Features.Progress.Commands.UpdateWatchTime;

public record UpdateWatchTimeCommand(
    int CourseId,
    int LessonId,
    int WatchedSeconds,   
    int TotalSeconds      
) : IRequest<UpdateWatchTimeResult>;

public record UpdateWatchTimeResult(
    int LessonId,
    int WatchedSeconds,
    double WatchedPercent,
    bool JustCompleted    
);