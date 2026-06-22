using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using TaskManagement.Application.Tasks;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Integration.Tasks;

public sealed class TasksApiTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task CreateTask_ReturnsCreatedTask()
    {
        using var factory = new TaskManagementApiFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var request = new CreateTaskRequest(
            "Prepare sprint planning",
            "Review backlog and define priorities.",
            DomainTaskStatus.Pending,
            new DateTime(2026, 7, 1, 14, 0, 0, DateTimeKind.Utc),
            TaskManagementApiFactory.SeedUserId);

        var response = await client.PostAsJsonAsync("/api/tasks", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions);
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Title.Should().Be(request.Title);
        body.Status.Should().Be(DomainTaskStatus.Pending);
        body.User.Email.Should().Be("ada@example.com");
    }

    [Fact]
    public async Task TaskLifecycle_SupportsGetUpdateGetByUserAndDelete()
    {
        using var factory = new TaskManagementApiFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/tasks",
            new CreateTaskRequest(
                "Draft release notes",
                null,
                DomainTaskStatus.Pending,
                new DateTime(2026, 7, 2, 12, 0, 0, DateTimeKind.Utc),
                TaskManagementApiFactory.SeedUserId),
            JsonOptions);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, await createResponse.Content.ReadAsStringAsync());
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions);

        var byId = await client.GetFromJsonAsync<TaskResponse>($"/api/tasks/{created!.Id}", JsonOptions);
        byId!.Title.Should().Be("Draft release notes");

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/tasks/{created.Id}",
            new UpdateTaskRequest(
                "Publish release notes",
                "Send the final version to stakeholders.",
                DomainTaskStatus.Completed,
                new DateTime(2026, 7, 3, 12, 0, 0, DateTimeKind.Utc),
                TaskManagementApiFactory.SeedUserId),
            JsonOptions);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TaskResponse>(JsonOptions);
        updated!.Status.Should().Be(DomainTaskStatus.Completed);

        var byUser = await client.GetFromJsonAsync<TaskResponse[]>(
            $"/api/tasks/by-user/{TaskManagementApiFactory.SeedUserId}",
            JsonOptions);

        byUser.Should().ContainSingle(task => task.Id == created.Id);

        var deleteResponse = await client.DeleteAsync($"/api/tasks/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var missingResponse = await client.GetAsync($"/api/tasks/{created.Id}");
        missingResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTask_WithUnknownUser_ReturnsNotFound()
    {
        using var factory = new TaskManagementApiFactory();
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/tasks",
            new CreateTaskRequest(
                "Unassigned task",
                null,
                DomainTaskStatus.Pending,
                DateTime.UtcNow,
                Guid.NewGuid()),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound, await response.Content.ReadAsStringAsync());
    }
}
