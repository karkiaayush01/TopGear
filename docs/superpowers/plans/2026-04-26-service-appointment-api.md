# Service Appointment API Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a fully-functional Service Appointment API (CRUD + cancel + reschedule) to the existing TopGear ASP.NET Core solution without altering any existing functionality.

**Architecture:** Follow the exact 4-layer Clean Architecture already in use — Domain entity → Application DTOs/Interfaces/Service → Infrastructure Repository → API Controller. All new code is purely additive: the only modifications to existing files are two-line additions (one DbSet, two DI registrations).

**Tech Stack:** .NET 10, ASP.NET Core, EF Core 10 + Npgsql, ASP.NET Core Identity, JWT Bearer auth, xUnit + Moq for tests.

---

## File Map

### New files (create)
| File | Responsibility |
|---|---|
| `TopGear.Domain/Enums/AppointmentStatus.cs` | Appointment lifecycle states |
| `TopGear.Domain/Entities/ServiceAppointment.cs` | Appointment domain entity |
| `TopGear.Application/DTOs/AppointmentDTO/CreateAppointmentDTO.cs` | Input: create |
| `TopGear.Application/DTOs/AppointmentDTO/UpdateAppointmentDTO.cs` | Input: general update (partial) |
| `TopGear.Application/DTOs/AppointmentDTO/CancelAppointmentDTO.cs` | Input: cancel |
| `TopGear.Application/DTOs/AppointmentDTO/RescheduleAppointmentDTO.cs` | Input: reschedule |
| `TopGear.Application/DTOs/AppointmentDTO/AppointmentResponseDTO.cs` | Output: all read/write responses |
| `TopGear.Application/Interfaces/IAppointmentRepository.cs` | Repository contract |
| `TopGear.Application/Interfaces/IAppointmentService.cs` | Service contract |
| `TopGear.Application/Services/AppointmentService.cs` | Business logic |
| `TopGear.Infrastructure/Repositories/AppointmentRepository.cs` | EF Core data access |
| `TopGear/Controllers/AppointmentController.cs` | HTTP endpoints |
| `TopGear.Tests/TopGear.Tests.csproj` | xUnit test project |
| `TopGear.Tests/AppointmentServiceTests.cs` | All service unit tests |

### Modified files (additions only)
| File | What is added |
|---|---|
| `TopGear.Infrastructure/Data/AppDbContext.cs` | One `DbSet<ServiceAppointment>` property |
| `TopGear.Infrastructure/DependencyInjections.cs` | Two `AddScoped` registrations |

---

## Task 1: Domain — Enum + Entity

**Files:**
- Create: `TopGear.Domain/Enums/AppointmentStatus.cs`
- Create: `TopGear.Domain/Entities/ServiceAppointment.cs`

- [ ] **Step 1: Create `AppointmentStatus.cs`**

```csharp
namespace TopGear.Domain.Enums;

public enum AppointmentStatus
{
    Scheduled = 1,
    Cancelled = 2,
    Completed = 3,
    Rescheduled = 4
}
```

- [ ] **Step 2: Create `ServiceAppointment.cs`**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class ServiceAppointment
{
    public Guid AppointmentId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(255)]
    public string ServiceType { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string VehicleInfo { get; set; } = null!;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? RescheduledFrom { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;
}
```

- [ ] **Step 3: Build Domain project to verify no errors**

Run: `dotnet build TopGear.Domain`

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add TopGear.Domain/Enums/AppointmentStatus.cs TopGear.Domain/Entities/ServiceAppointment.cs
git commit -m "feat: add ServiceAppointment entity and AppointmentStatus enum"
```

---

## Task 2: DTOs

**Files:**
- Create: `TopGear.Application/DTOs/AppointmentDTO/AppointmentResponseDTO.cs`
- Create: `TopGear.Application/DTOs/AppointmentDTO/CreateAppointmentDTO.cs`
- Create: `TopGear.Application/DTOs/AppointmentDTO/UpdateAppointmentDTO.cs`
- Create: `TopGear.Application/DTOs/AppointmentDTO/CancelAppointmentDTO.cs`
- Create: `TopGear.Application/DTOs/AppointmentDTO/RescheduleAppointmentDTO.cs`

- [ ] **Step 1: Create `AppointmentResponseDTO.cs`**

```csharp
using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.AppointmentDTO;

public class AppointmentResponseDTO
{
    public Guid AppointmentId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateTime AppointmentDate { get; set; }
    public string ServiceType { get; set; } = null!;
    public string VehicleInfo { get; set; } = null!;
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? RescheduledFrom { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

- [ ] **Step 2: Create `CreateAppointmentDTO.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.AppointmentDTO;

public class CreateAppointmentDTO
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(255)]
    public string ServiceType { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string VehicleInfo { get; set; } = null!;

    public string? Notes { get; set; }
}
```

- [ ] **Step 3: Create `UpdateAppointmentDTO.cs`**

```csharp
namespace TopGear.Application.DTOs.AppointmentDTO;

public class UpdateAppointmentDTO
{
    public DateTime? AppointmentDate { get; set; }
    public string? ServiceType { get; set; }
    public string? VehicleInfo { get; set; }
    public string? Notes { get; set; }
}
```

- [ ] **Step 4: Create `CancelAppointmentDTO.cs`**

```csharp
namespace TopGear.Application.DTOs.AppointmentDTO;

public class CancelAppointmentDTO
{
    public string? CancellationReason { get; set; }
}
```

- [ ] **Step 5: Create `RescheduleAppointmentDTO.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.AppointmentDTO;

public class RescheduleAppointmentDTO
{
    [Required]
    public DateTime NewAppointmentDate { get; set; }

    public string? Notes { get; set; }
}
```

- [ ] **Step 6: Build Application project to verify no errors**

Run: `dotnet build TopGear.Application`

Expected: `Build succeeded.`

- [ ] **Step 7: Commit**

```bash
git add TopGear.Application/DTOs/AppointmentDTO/
git commit -m "feat: add appointment DTOs (create, update, cancel, reschedule, response)"
```

---

## Task 3: Interfaces

**Files:**
- Create: `TopGear.Application/Interfaces/IAppointmentRepository.cs`
- Create: `TopGear.Application/Interfaces/IAppointmentService.cs`

- [ ] **Step 1: Create `IAppointmentRepository.cs`**

```csharp
using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IAppointmentRepository : IRepositoryBase<ServiceAppointment>
{
    Task<IEnumerable<ServiceAppointment>> GetAllWithCustomerAsync();
    Task<IEnumerable<ServiceAppointment>> GetByCustomerIdAsync(Guid customerId);
    Task<ServiceAppointment?> GetByIdWithCustomerAsync(Guid appointmentId);
}
```

- [ ] **Step 2: Create `IAppointmentService.cs`**

```csharp
using TopGear.Application.DTOs.AppointmentDTO;

namespace TopGear.Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponseDTO>> GetAllAsync();
    Task<IEnumerable<AppointmentResponseDTO>> GetByCustomerIdAsync(Guid customerId);
    Task<AppointmentResponseDTO> GetByIdAsync(Guid appointmentId);
    Task<AppointmentResponseDTO> CreateAsync(CreateAppointmentDTO dto);
    Task<AppointmentResponseDTO?> UpdateAsync(Guid appointmentId, UpdateAppointmentDTO dto);
    Task CancelAsync(Guid appointmentId, CancelAppointmentDTO dto);
    Task RescheduleAsync(Guid appointmentId, RescheduleAppointmentDTO dto);
    Task<bool> DeleteAsync(Guid appointmentId);
}
```

- [ ] **Step 3: Build Application project to verify no errors**

Run: `dotnet build TopGear.Application`

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add TopGear.Application/Interfaces/IAppointmentRepository.cs TopGear.Application/Interfaces/IAppointmentService.cs
git commit -m "feat: add IAppointmentRepository and IAppointmentService interfaces"
```

---

## Task 4: Test Project Setup + All Tests (Red Phase)

**Files:**
- Create: `TopGear.Tests/TopGear.Tests.csproj`
- Create: `TopGear.Tests/AppointmentServiceTests.cs`
- Create: `TopGear.Application/Services/AppointmentService.cs` (stub — all methods throw `NotImplementedException`)

- [ ] **Step 1: Create `TopGear.Tests/TopGear.Tests.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\TopGear.Application\TopGear.Application.csproj" />
    <ProjectReference Include="..\TopGear.Domain\TopGear.Domain.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Add the test project to the solution**

Run from the solution root (`E:\Computing\Fifth Semister\Application Development\TopGear\TopGear\`):

```bash
dotnet sln add TopGear.Tests/TopGear.Tests.csproj
```

Expected: `Project 'TopGear.Tests\TopGear.Tests.csproj' added to the solution.`

- [ ] **Step 3: Create stub `AppointmentService.cs`**

This stub allows the test file to compile. Every method throws so all tests fail red.

```csharp
using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.AppointmentDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Application.Services;

public class AppointmentService(IAppointmentRepository repository, ILogger<AppointmentService> logger) : IAppointmentService
{
    public Task<IEnumerable<AppointmentResponseDTO>> GetAllAsync() => throw new NotImplementedException();
    public Task<IEnumerable<AppointmentResponseDTO>> GetByCustomerIdAsync(Guid customerId) => throw new NotImplementedException();
    public Task<AppointmentResponseDTO> GetByIdAsync(Guid appointmentId) => throw new NotImplementedException();
    public Task<AppointmentResponseDTO> CreateAsync(CreateAppointmentDTO dto) => throw new NotImplementedException();
    public Task<AppointmentResponseDTO?> UpdateAsync(Guid appointmentId, UpdateAppointmentDTO dto) => throw new NotImplementedException();
    public Task CancelAsync(Guid appointmentId, CancelAppointmentDTO dto) => throw new NotImplementedException();
    public Task RescheduleAsync(Guid appointmentId, RescheduleAppointmentDTO dto) => throw new NotImplementedException();
    public Task<bool> DeleteAsync(Guid appointmentId) => throw new NotImplementedException();
}
```

- [ ] **Step 4: Create `TopGear.Tests/AppointmentServiceTests.cs`** (all tests)

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using TopGear.Application.DTOs.AppointmentDTO;
using TopGear.Application.Interfaces;
using TopGear.Application.Services;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Tests;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _mockRepo;
    private readonly Mock<ILogger<AppointmentService>> _mockLogger;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _mockRepo = new Mock<IAppointmentRepository>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();
        _service = new AppointmentService(_mockRepo.Object, _mockLogger.Object);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsResponseWithScheduledStatus()
    {
        var dto = new CreateAppointmentDTO
        {
            CustomerId = Guid.NewGuid(),
            AppointmentDate = DateTime.UtcNow.AddDays(3),
            ServiceType = "Oil Change",
            VehicleInfo = "2020 Toyota Camry"
        };
        _mockRepo.Setup(r => r.Create(It.IsAny<ServiceAppointment>()));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.CustomerId, result.CustomerId);
        Assert.Equal(dto.ServiceType, result.ServiceType);
        Assert.Equal(dto.VehicleInfo, result.VehicleInfo);
        Assert.Equal(AppointmentStatus.Scheduled, result.Status);
        _mockRepo.Verify(r => r.Create(It.IsAny<ServiceAppointment>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsAllAppointments()
    {
        var customer = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var appointments = new List<ServiceAppointment>
        {
            new() { AppointmentId = Guid.NewGuid(), CustomerId = customer.Id, ServiceType = "Oil Change",   VehicleInfo = "Car",   Status = AppointmentStatus.Scheduled, AppointmentDate = DateTime.UtcNow.AddDays(1), Customer = customer },
            new() { AppointmentId = Guid.NewGuid(), CustomerId = customer.Id, ServiceType = "Tire Change",  VehicleInfo = "Truck", Status = AppointmentStatus.Scheduled, AppointmentDate = DateTime.UtcNow.AddDays(2), Customer = customer }
        };
        _mockRepo.Setup(r => r.GetAllWithCustomerAsync()).ReturnsAsync(appointments);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsResponseWithCustomerName()
    {
        var id = Guid.NewGuid();
        var customer = new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = customer.Id,
            ServiceType = "Brake Check",
            VehicleInfo = "2021 Honda Civic",
            Status = AppointmentStatus.Scheduled,
            AppointmentDate = DateTime.UtcNow.AddDays(5),
            Customer = customer
        };
        _mockRepo.Setup(r => r.GetByIdWithCustomerAsync(id)).ReturnsAsync(appointment);

        var result = await _service.GetByIdAsync(id);

        Assert.Equal(id, result.AppointmentId);
        Assert.Equal("Jane Smith", result.CustomerName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdWithCustomerAsync(id)).ReturnsAsync((ServiceAppointment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    // --- GetByCustomerIdAsync ---

    [Fact]
    public async Task GetByCustomerIdAsync_ReturnsOnlyThatCustomersAppointments()
    {
        var customerId = Guid.NewGuid();
        var customer = new User { Id = customerId, FirstName = "Alice", LastName = "Brown" };
        var appointments = new List<ServiceAppointment>
        {
            new() { AppointmentId = Guid.NewGuid(), CustomerId = customerId, ServiceType = "Checkup", VehicleInfo = "SUV", Status = AppointmentStatus.Scheduled, AppointmentDate = DateTime.UtcNow.AddDays(1), Customer = customer }
        };
        _mockRepo.Setup(r => r.GetByCustomerIdAsync(customerId)).ReturnsAsync(appointments);

        var result = await _service.GetByCustomerIdAsync(customerId);

        Assert.Single(result);
        Assert.Equal(customerId, result.First().CustomerId);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ExistingId_ReturnsUpdatedResponse()
    {
        var id = Guid.NewGuid();
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = Guid.NewGuid(),
            ServiceType = "Oil Change",
            VehicleInfo = "Sedan",
            Status = AppointmentStatus.Scheduled,
            AppointmentDate = DateTime.UtcNow.AddDays(2)
        };
        var dto = new UpdateAppointmentDTO { ServiceType = "Tire Change" };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);
        _mockRepo.Setup(r => r.Update(It.IsAny<ServiceAppointment>()));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(id, dto);

        Assert.NotNull(result);
        Assert.Equal("Tire Change", result!.ServiceType);
        _mockRepo.Verify(r => r.Update(It.IsAny<ServiceAppointment>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ServiceAppointment?)null);

        var result = await _service.UpdateAsync(id, new UpdateAppointmentDTO());

        Assert.Null(result);
    }

    // --- CancelAsync ---

    [Fact]
    public async Task CancelAsync_ScheduledAppointment_SetsStatusToCancelledWithReason()
    {
        var id = Guid.NewGuid();
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = Guid.NewGuid(),
            ServiceType = "Oil Change",
            VehicleInfo = "Car",
            Status = AppointmentStatus.Scheduled,
            AppointmentDate = DateTime.UtcNow.AddDays(3)
        };
        var dto = new CancelAppointmentDTO { CancellationReason = "Customer request" };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);
        _mockRepo.Setup(r => r.Update(It.IsAny<ServiceAppointment>()));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.CancelAsync(id, dto);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal("Customer request", appointment.CancellationReason);
        _mockRepo.Verify(r => r.Update(It.IsAny<ServiceAppointment>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_AlreadyCancelledAppointment_ThrowsArgumentException()
    {
        var id = Guid.NewGuid();
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = Guid.NewGuid(),
            ServiceType = "Oil Change",
            VehicleInfo = "Car",
            Status = AppointmentStatus.Cancelled,
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CancelAsync(id, new CancelAppointmentDTO()));
    }

    [Fact]
    public async Task CancelAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ServiceAppointment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CancelAsync(id, new CancelAppointmentDTO()));
    }

    // --- RescheduleAsync ---

    [Fact]
    public async Task RescheduleAsync_ScheduledAppointment_UpdatesDateAndSetsRescheduledStatus()
    {
        var id = Guid.NewGuid();
        var originalDate = DateTime.UtcNow.AddDays(2);
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = Guid.NewGuid(),
            ServiceType = "Oil Change",
            VehicleInfo = "Car",
            Status = AppointmentStatus.Scheduled,
            AppointmentDate = originalDate
        };
        var newDate = DateTime.UtcNow.AddDays(7);
        var dto = new RescheduleAppointmentDTO { NewAppointmentDate = newDate };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);
        _mockRepo.Setup(r => r.Update(It.IsAny<ServiceAppointment>()));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.RescheduleAsync(id, dto);

        Assert.Equal(newDate, appointment.AppointmentDate);
        Assert.Equal(originalDate, appointment.RescheduledFrom);
        Assert.Equal(AppointmentStatus.Rescheduled, appointment.Status);
        _mockRepo.Verify(r => r.Update(It.IsAny<ServiceAppointment>()), Times.Once);
    }

    [Fact]
    public async Task RescheduleAsync_CancelledAppointment_ThrowsArgumentException()
    {
        var id = Guid.NewGuid();
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = Guid.NewGuid(),
            ServiceType = "Oil Change",
            VehicleInfo = "Car",
            Status = AppointmentStatus.Cancelled,
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RescheduleAsync(id, new RescheduleAppointmentDTO { NewAppointmentDate = DateTime.UtcNow.AddDays(5) }));
    }

    [Fact]
    public async Task RescheduleAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ServiceAppointment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.RescheduleAsync(id, new RescheduleAppointmentDTO { NewAppointmentDate = DateTime.UtcNow.AddDays(5) }));
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrueAndCallsDelete()
    {
        var id = Guid.NewGuid();
        var appointment = new ServiceAppointment
        {
            AppointmentId = id,
            CustomerId = Guid.NewGuid(),
            ServiceType = "Oil Change",
            VehicleInfo = "Car",
            Status = AppointmentStatus.Scheduled,
            AppointmentDate = DateTime.UtcNow.AddDays(2)
        };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appointment);
        _mockRepo.Setup(r => r.Delete(It.IsAny<ServiceAppointment>()));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(id);

        Assert.True(result);
        _mockRepo.Verify(r => r.Delete(It.IsAny<ServiceAppointment>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ServiceAppointment?)null);

        var result = await _service.DeleteAsync(id);

        Assert.False(result);
    }
}
```

- [ ] **Step 5: Run all tests — confirm they all fail red**

Run: `dotnet test TopGear.Tests`

Expected: All 15 tests FAIL with `NotImplementedException`. Zero passing. This is correct — the stub is not implemented.

- [ ] **Step 6: Commit**

```bash
git add TopGear.Tests/ TopGear.Application/Services/AppointmentService.cs
git commit -m "test: add AppointmentService unit tests (all red), add stub service"
```

---

## Task 5: Implement `CreateAsync` (Green)

**File:** `TopGear.Application/Services/AppointmentService.cs`

- [ ] **Step 1: Replace the `CreateAsync` stub with real implementation**

Replace only the `CreateAsync` method body in `AppointmentService.cs`:

```csharp
public async Task<AppointmentResponseDTO> CreateAsync(CreateAppointmentDTO dto)
{
    try
    {
        logger.LogInformation("Creating appointment for customer: {CustomerId}", dto.CustomerId);

        var appointment = new ServiceAppointment
        {
            CustomerId = dto.CustomerId,
            AppointmentDate = dto.AppointmentDate,
            ServiceType = dto.ServiceType,
            VehicleInfo = dto.VehicleInfo,
            Notes = dto.Notes,
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        repository.Create(appointment);
        await repository.SaveChangesAsync();

        logger.LogInformation("Appointment created with ID: {AppointmentId}", appointment.AppointmentId);

        return new AppointmentResponseDTO
        {
            AppointmentId = appointment.AppointmentId,
            CustomerId = appointment.CustomerId,
            CustomerName = "",
            AppointmentDate = appointment.AppointmentDate,
            ServiceType = appointment.ServiceType,
            VehicleInfo = appointment.VehicleInfo,
            Status = appointment.Status,
            Notes = appointment.Notes,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating appointment");
        throw;
    }
}
```

You will also need these using statements at the top of `AppointmentService.cs`:

```csharp
using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.AppointmentDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;
```

- [ ] **Step 2: Run only the CreateAsync test — confirm it passes**

Run: `dotnet test TopGear.Tests --filter "CreateAsync"`

Expected: 1 test PASSES. All others still fail.

- [ ] **Step 3: Commit**

```bash
git add TopGear.Application/Services/AppointmentService.cs
git commit -m "feat: implement AppointmentService.CreateAsync"
```

---

## Task 6: Implement Read Methods (Green)

**File:** `TopGear.Application/Services/AppointmentService.cs`

Add a private helper `MapToResponse` and implement the three read methods.

- [ ] **Step 1: Add the `MapToResponse` private helper at the bottom of the class**

```csharp
private static AppointmentResponseDTO MapToResponse(ServiceAppointment a) => new()
{
    AppointmentId = a.AppointmentId,
    CustomerId = a.CustomerId,
    CustomerName = a.Customer != null ? $"{a.Customer.FirstName} {a.Customer.LastName}" : "",
    AppointmentDate = a.AppointmentDate,
    ServiceType = a.ServiceType,
    VehicleInfo = a.VehicleInfo,
    Status = a.Status,
    Notes = a.Notes,
    CancellationReason = a.CancellationReason,
    RescheduledFrom = a.RescheduledFrom,
    CreatedAt = a.CreatedAt,
    UpdatedAt = a.UpdatedAt
};
```

- [ ] **Step 2: Replace `GetAllAsync` stub**

```csharp
public async Task<IEnumerable<AppointmentResponseDTO>> GetAllAsync()
{
    logger.LogInformation("Fetching all appointments");
    var appointments = await repository.GetAllWithCustomerAsync();
    return appointments.Select(MapToResponse);
}
```

- [ ] **Step 3: Replace `GetByCustomerIdAsync` stub**

```csharp
public async Task<IEnumerable<AppointmentResponseDTO>> GetByCustomerIdAsync(Guid customerId)
{
    logger.LogInformation("Fetching appointments for customer: {CustomerId}", customerId);
    var appointments = await repository.GetByCustomerIdAsync(customerId);
    return appointments.Select(MapToResponse);
}
```

- [ ] **Step 4: Replace `GetByIdAsync` stub**

```csharp
public async Task<AppointmentResponseDTO> GetByIdAsync(Guid appointmentId)
{
    logger.LogInformation("Fetching appointment with ID: {AppointmentId}", appointmentId);
    var appointment = await repository.GetByIdWithCustomerAsync(appointmentId);
    if (appointment == null)
    {
        logger.LogWarning("Appointment not found with ID: {AppointmentId}", appointmentId);
        throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");
    }
    return MapToResponse(appointment);
}
```

- [ ] **Step 5: Run read method tests — confirm they pass**

Run: `dotnet test TopGear.Tests --filter "GetAll|GetById|GetByCustomer"`

Expected: 4 tests PASS.

- [ ] **Step 6: Commit**

```bash
git add TopGear.Application/Services/AppointmentService.cs
git commit -m "feat: implement AppointmentService read methods (GetAll, GetById, GetByCustomerId)"
```

---

## Task 7: Implement `UpdateAsync` (Green)

**File:** `TopGear.Application/Services/AppointmentService.cs`

- [ ] **Step 1: Replace `UpdateAsync` stub**

```csharp
public async Task<AppointmentResponseDTO?> UpdateAsync(Guid appointmentId, UpdateAppointmentDTO dto)
{
    logger.LogInformation("Updating appointment with ID: {AppointmentId}", appointmentId);

    var appointment = await repository.GetByIdAsync(appointmentId);
    if (appointment == null)
    {
        logger.LogWarning("Update failed. Appointment not found with ID: {AppointmentId}", appointmentId);
        return null;
    }

    appointment.AppointmentDate = dto.AppointmentDate ?? appointment.AppointmentDate;
    appointment.ServiceType = string.IsNullOrWhiteSpace(dto.ServiceType) ? appointment.ServiceType : dto.ServiceType;
    appointment.VehicleInfo = string.IsNullOrWhiteSpace(dto.VehicleInfo) ? appointment.VehicleInfo : dto.VehicleInfo;
    appointment.Notes = dto.Notes ?? appointment.Notes;
    appointment.UpdatedAt = DateTime.UtcNow;

    repository.Update(appointment);
    await repository.SaveChangesAsync();

    logger.LogInformation("Appointment updated with ID: {AppointmentId}", appointmentId);

    return new AppointmentResponseDTO
    {
        AppointmentId = appointment.AppointmentId,
        CustomerId = appointment.CustomerId,
        CustomerName = "",
        AppointmentDate = appointment.AppointmentDate,
        ServiceType = appointment.ServiceType,
        VehicleInfo = appointment.VehicleInfo,
        Status = appointment.Status,
        Notes = appointment.Notes,
        CancellationReason = appointment.CancellationReason,
        RescheduledFrom = appointment.RescheduledFrom,
        CreatedAt = appointment.CreatedAt,
        UpdatedAt = appointment.UpdatedAt
    };
}
```

- [ ] **Step 2: Run UpdateAsync tests — confirm they pass**

Run: `dotnet test TopGear.Tests --filter "UpdateAsync"`

Expected: 2 tests PASS.

- [ ] **Step 3: Commit**

```bash
git add TopGear.Application/Services/AppointmentService.cs
git commit -m "feat: implement AppointmentService.UpdateAsync"
```

---

## Task 8: Implement `CancelAsync` (Green)

**File:** `TopGear.Application/Services/AppointmentService.cs`

- [ ] **Step 1: Replace `CancelAsync` stub**

```csharp
public async Task CancelAsync(Guid appointmentId, CancelAppointmentDTO dto)
{
    logger.LogInformation("Cancelling appointment with ID: {AppointmentId}", appointmentId);

    var appointment = await repository.GetByIdAsync(appointmentId);
    if (appointment == null)
    {
        logger.LogWarning("Cancel failed. Appointment not found with ID: {AppointmentId}", appointmentId);
        throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");
    }

    if (appointment.Status == AppointmentStatus.Cancelled)
        throw new ArgumentException("Appointment is already cancelled.");

    appointment.Status = AppointmentStatus.Cancelled;
    appointment.CancellationReason = dto.CancellationReason;
    appointment.UpdatedAt = DateTime.UtcNow;

    repository.Update(appointment);
    await repository.SaveChangesAsync();

    logger.LogInformation("Appointment cancelled with ID: {AppointmentId}", appointmentId);
}
```

- [ ] **Step 2: Run CancelAsync tests — confirm they pass**

Run: `dotnet test TopGear.Tests --filter "CancelAsync"`

Expected: 3 tests PASS.

- [ ] **Step 3: Commit**

```bash
git add TopGear.Application/Services/AppointmentService.cs
git commit -m "feat: implement AppointmentService.CancelAsync"
```

---

## Task 9: Implement `RescheduleAsync` (Green)

**File:** `TopGear.Application/Services/AppointmentService.cs`

- [ ] **Step 1: Replace `RescheduleAsync` stub**

```csharp
public async Task RescheduleAsync(Guid appointmentId, RescheduleAppointmentDTO dto)
{
    logger.LogInformation("Rescheduling appointment with ID: {AppointmentId}", appointmentId);

    var appointment = await repository.GetByIdAsync(appointmentId);
    if (appointment == null)
    {
        logger.LogWarning("Reschedule failed. Appointment not found with ID: {AppointmentId}", appointmentId);
        throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");
    }

    if (appointment.Status == AppointmentStatus.Cancelled)
        throw new ArgumentException("Cannot reschedule a cancelled appointment.");

    appointment.RescheduledFrom = appointment.AppointmentDate;
    appointment.AppointmentDate = dto.NewAppointmentDate;
    appointment.Notes = dto.Notes ?? appointment.Notes;
    appointment.Status = AppointmentStatus.Rescheduled;
    appointment.UpdatedAt = DateTime.UtcNow;

    repository.Update(appointment);
    await repository.SaveChangesAsync();

    logger.LogInformation("Appointment rescheduled with ID: {AppointmentId}", appointmentId);
}
```

- [ ] **Step 2: Run RescheduleAsync tests — confirm they pass**

Run: `dotnet test TopGear.Tests --filter "RescheduleAsync"`

Expected: 3 tests PASS.

- [ ] **Step 3: Commit**

```bash
git add TopGear.Application/Services/AppointmentService.cs
git commit -m "feat: implement AppointmentService.RescheduleAsync"
```

---

## Task 10: Implement `DeleteAsync` (Green)

**File:** `TopGear.Application/Services/AppointmentService.cs`

- [ ] **Step 1: Replace `DeleteAsync` stub**

```csharp
public async Task<bool> DeleteAsync(Guid appointmentId)
{
    logger.LogInformation("Deleting appointment with ID: {AppointmentId}", appointmentId);

    var appointment = await repository.GetByIdAsync(appointmentId);
    if (appointment == null)
    {
        logger.LogWarning("Delete failed. Appointment not found with ID: {AppointmentId}", appointmentId);
        return false;
    }

    repository.Delete(appointment);
    await repository.SaveChangesAsync();

    logger.LogInformation("Appointment deleted with ID: {AppointmentId}", appointmentId);
    return true;
}
```

- [ ] **Step 2: Run ALL tests — confirm all 15 pass**

Run: `dotnet test TopGear.Tests`

Expected: **15 tests PASS. 0 failures.**

- [ ] **Step 3: Commit**

```bash
git add TopGear.Application/Services/AppointmentService.cs
git commit -m "feat: implement AppointmentService.DeleteAsync — all 15 tests green"
```

---

## Task 11: Repository Implementation + DbContext

**Files:**
- Create: `TopGear.Infrastructure/Repositories/AppointmentRepository.cs`
- Modify: `TopGear.Infrastructure/Data/AppDbContext.cs` (add one line)

- [ ] **Step 1: Create `AppointmentRepository.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class AppointmentRepository(AppDbContext context)
    : RepositoryBase<ServiceAppointment>(context), IAppointmentRepository
{
    public async Task<IEnumerable<ServiceAppointment>> GetAllWithCustomerAsync() =>
        await Context.Set<ServiceAppointment>()
            .Include(a => a.Customer)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<ServiceAppointment>> GetByCustomerIdAsync(Guid customerId) =>
        await Context.Set<ServiceAppointment>()
            .Where(a => a.CustomerId == customerId)
            .Include(a => a.Customer)
            .AsNoTracking()
            .ToListAsync();

    public async Task<ServiceAppointment?> GetByIdWithCustomerAsync(Guid appointmentId) =>
        await Context.Set<ServiceAppointment>()
            .Include(a => a.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
}
```

- [ ] **Step 2: Add `DbSet` to `AppDbContext.cs`**

Open `TopGear.Infrastructure/Data/AppDbContext.cs`.

Add this line after the existing `DbSet` properties (after `public DbSet<Vendor> Vendors { get; set; } = null!;`):

```csharp
public DbSet<ServiceAppointment> Appointments { get; set; } = null!;
```

The full file after the change:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TopGear.Domain.Entities;

namespace TopGear.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options): IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<User> AppUsers { get; set; } = null!;
    public DbSet<Part> Parts { get; set; } = null!;
    public DbSet<Vendor> Vendors { get; set; } = null!;
    public DbSet<ServiceAppointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("topgear");
    }
}
```

- [ ] **Step 3: Build Infrastructure project to verify no errors**

Run: `dotnet build TopGear.Infrastructure`

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add TopGear.Infrastructure/Repositories/AppointmentRepository.cs TopGear.Infrastructure/Data/AppDbContext.cs
git commit -m "feat: add AppointmentRepository and register ServiceAppointment DbSet"
```

---

## Task 12: DI Registration + Database Migration

**Files:**
- Modify: `TopGear.Infrastructure/DependencyInjections.cs` (add two lines)

- [ ] **Step 1: Add registrations to `DependencyInjections.cs`**

Open `TopGear.Infrastructure/DependencyInjections.cs`.

Add under `//repository injections`:
```csharp
services.AddScoped<IAppointmentRepository, AppointmentRepository>();
```

Add under `//services injections`:
```csharp
services.AddScoped<IAppointmentService, AppointmentService>();
```

The full file after changes:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopGear.Application.Interfaces;
using TopGear.Application.Services;
using TopGear.Infrastructure.Data;
using TopGear.Infrastructure.Repositories;

namespace TopGear.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        services.AddScoped<AppDbSeeder>();

        //repository injections
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        //services injections
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        return services;
    }
}
```

- [ ] **Step 2: Build the full solution**

Run: `dotnet build`

Expected: `Build succeeded.` for all projects.

- [ ] **Step 3: Create the EF Core migration**

Run from the solution root:

```bash
dotnet ef migrations add AddServiceAppointments --project TopGear.Infrastructure --startup-project TopGear
```

Expected: A new migration file appears in `TopGear.Infrastructure/Migrations/` named `<timestamp>_AddServiceAppointments.cs` containing `CreateTable` for `Appointments`.

- [ ] **Step 4: Apply migration to database**

```bash
dotnet ef database update --project TopGear.Infrastructure --startup-project TopGear
```

Expected: `Done.` — The `topgear.Appointments` table now exists in Supabase.

- [ ] **Step 5: Commit**

```bash
git add TopGear.Infrastructure/DependencyInjections.cs TopGear.Infrastructure/Migrations/
git commit -m "feat: register appointment DI, add AddServiceAppointments migration"
```

---

## Task 13: Controller

**Files:**
- Create: `TopGear/Controllers/AppointmentController.cs`

- [ ] **Step 1: Create `AppointmentController.cs`**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.AppointmentDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/appointment")]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var appointments = await appointmentService.GetByCustomerIdAsync(customerId);
        return Ok(appointments);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await appointmentService.GetByIdAsync(id);
        return Ok(appointment);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO dto)
    {
        var created = await appointmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, created);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentDTO dto)
    {
        var updated = await appointmentService.UpdateAsync(id, dto);
        if (updated == null)
            throw new KeyNotFoundException();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentDTO dto)
    {
        await appointmentService.CancelAsync(id, dto);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPatch("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentDTO dto)
    {
        await appointmentService.RescheduleAsync(id, dto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await appointmentService.DeleteAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }
}
```

- [ ] **Step 2: Build the full solution one final time**

Run: `dotnet build`

Expected: `Build succeeded.` — zero errors.

- [ ] **Step 3: Run all tests**

Run: `dotnet test TopGear.Tests`

Expected: **15 tests PASS. 0 failures.**

- [ ] **Step 4: Smoke test via Swagger**

Run the API: `dotnet run --project TopGear`

Open: `https://localhost:7130/swagger`

Verify all 8 appointment endpoints appear under the `Appointment` group:
- `GET  /api/appointment`
- `GET  /api/appointment/customer/{customerId}`
- `GET  /api/appointment/{id}`
- `POST /api/appointment`
- `PUT  /api/appointment/{id}`
- `PATCH /api/appointment/{id}/cancel`
- `PATCH /api/appointment/{id}/reschedule`
- `DELETE /api/appointment/{id}`

- [ ] **Step 5: Final commit**

```bash
git add TopGear/Controllers/AppointmentController.cs
git commit -m "feat: add AppointmentController — Service Appointment API complete"
```

---

## Summary of All Endpoints

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `api/appointment` | Admin, Staff | Get all appointments |
| `GET` | `api/appointment/customer/{customerId}` | Admin, Staff | Get appointments by customer |
| `GET` | `api/appointment/{id}` | Any authenticated | Get single appointment |
| `POST` | `api/appointment` | Any authenticated | Create appointment |
| `PUT` | `api/appointment/{id}` | Admin, Staff | Update appointment details |
| `PATCH` | `api/appointment/{id}/cancel` | Admin, Staff | Cancel appointment |
| `PATCH` | `api/appointment/{id}/reschedule` | Admin, Staff | Reschedule appointment |
| `DELETE` | `api/appointment/{id}` | Admin only | Delete appointment |

## Summary of Business Rules
- Cancel throws `ArgumentException` if already `Cancelled` (→ 400 via GlobalExceptionHandler)
- Reschedule throws `ArgumentException` if already `Cancelled` (→ 400)
- Not found on any operation throws `KeyNotFoundException` (→ 404)
- `RescheduledFrom` stores the original date when rescheduling
- `Status` automatically transitions: Scheduled → Cancelled / Rescheduled
