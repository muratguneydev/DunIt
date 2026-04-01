namespace DunIt.Testing;

using AutoFixture;
using AutoFixture.Kernel;
using DunIt.Core.Schedules;

public class DunItFixture : Fixture
{
    public DunItFixture()
    {
        this.Customizations.Add(new TypeRelay(typeof(ChoreSchedule), typeof(DailySchedule)));
    }
}
