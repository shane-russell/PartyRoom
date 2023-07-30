using Moq;

namespace PartyRoom.Tests.DataAccessLayer;

[TestFixture]
public class MovementRepositoryTests
{
    private Mock<PartyRoomDbContext> _mockContext;
    private Mock<DbSet<Movement>> _mockMovementsDbSet;
    private MovementRepository _movementRepository;

    [SetUp]
    public void SetUp()
    {
        _mockMovementsDbSet = new Mock<DbSet<Movement>>();
        _mockContext = new Mock<PartyRoomDbContext>();
        _mockContext.Setup(ctx => ctx.Movements).Returns(_mockMovementsDbSet.Object);
        _movementRepository = new MovementRepository(_mockContext.Object);

        var now = DateTime.Now;
        var movements = new List<Movement>
        {
            new Movement { Id = 1, PartyRoomId = 1, NationalNumber = "12345678901", MovementType = MovementType.In, Time = now }, // today's movement
            new Movement { Id = 2, PartyRoomId = 1, NationalNumber = "12345678902", MovementType = MovementType.Out, Time = now.AddDays(-1) }, // yesterday's movement
        }.AsQueryable();
        

        // === SETTING UP LINQ (only sync methods)
        // Where(), Select(), GroupBy(), OrderBy(), ThenBy(), FirstOrDefault(),
        // SingleOrDefault(), ToList(), Any(), All(), Count(),
        // LongCount(), Sum(), Average(), Min(), Max()
        _mockMovementsDbSet.As<IQueryable<Movement>>().Setup(m => m.Provider).Returns(movements.Provider);
        _mockMovementsDbSet.As<IQueryable<Movement>>().Setup(m => m.Expression).Returns(movements.Expression);
        _mockMovementsDbSet.As<IQueryable<Movement>>().Setup(m => m.ElementType).Returns(movements.ElementType);
        _mockMovementsDbSet.As<IQueryable<Movement>>().Setup(m => m.GetEnumerator()).Returns(movements.GetEnumerator());
    }

    [Test]
    public void GetMovementsOfToday_ReturnsCorrectMovements()
    {
        // Arrange
        // LINQ mock 'Where()' and others are setup in setup method above

        // Act
        var result = _movementRepository.GetMovementsOfToday();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Exactly(1).Items);
        Assert.That(result.First().Id, Is.EqualTo(1));
    }

    [Test]
    public void GetAll_ReturnsAllMovements()
    {
        // Arrange
        // LINQ mock 'ToList()' and others are setup in setup method above

        // Act
        var result = _movementRepository.GetAll();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Exactly(2).Items);
        Assert.That(result.Select(m => m.Id), Is.EquivalentTo(new[] { 1, 2 }));
    }

    [Test]
    public void GetById_ReturnsCorrectMovement()
    {
        // Arrange
        _mockMovementsDbSet.Setup(m => m.Find(It.IsAny<int>())).Returns(new Movement { Id = 1, PartyRoomId = 1, NationalNumber = "12345678901", MovementType = MovementType.In, Time = DateTime.Now });
        
        // Act
        var result = _movementRepository.GetById(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void AddAsync_AddsMovement()
    {
        // Arrange
        var movement = new Movement { Id = 1, PartyRoomId = 1, NationalNumber = "12345678901", MovementType = MovementType.In, Time = DateTime.Now };
        _mockMovementsDbSet.Setup(m => m.Add(It.IsAny<Movement>())).Returns(_mockContext.Object.Entry(movement));
        _mockContext.Setup(c => c.SaveChanges()).Returns(1);
        
        // Act
        _movementRepository.Add(movement);

        // Assert
        _mockMovementsDbSet.Verify(m => m.Add(It.IsAny<Movement>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_UpdatesMovement()
    {
        // Arrange
        var movement = new Movement { Id = 1, PartyRoomId = 1, NationalNumber = "12345678901", MovementType = MovementType.In, Time = DateTime.Now };
        _mockMovementsDbSet.Setup(m => m.Update(It.IsAny<Movement>())).Returns(_mockContext.Object.Entry(movement));
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var repository = new MovementRepository(_mockContext.Object);

        // Act
        repository.Update(movement);

        // Assert
        _mockMovementsDbSet.Verify(m => m.Update(It.IsAny<Movement>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Delete_DeletesMovement()
    {
        // Arrange
        var movement = new Movement { Id = 1, PartyRoomId = 1, NationalNumber = "12345678901", MovementType = MovementType.In, Time = DateTime.Now };
        //_mockMovementsDbSet.Setup(m => m.Remove(It.IsAny<Movement>())).Returns(movement);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var repository = new MovementRepository(_mockContext.Object);

        // Act
        repository.Delete(movement);

        // Assert
        _mockMovementsDbSet.Verify(m => m.Remove(It.IsAny<Movement>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}