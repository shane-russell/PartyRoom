using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using PartyRoom.Domain;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using NuGet.Frameworks;

namespace PartyRoom.Tests.DataAccessLayer;

[TestFixture]
public class RoomRepositoryTests
{
    private Mock<PartyRoomDbContext> _mockPartyRoomDbContext;
    private RoomRepository _roomRepository;
    private Mock<DbSet<Room>> _mockRoomsDbSet;
    private List<Room> _rooms;

    [SetUp]
    public void SetUp()
    {
        _mockPartyRoomDbContext = new Mock<PartyRoomDbContext>();
        _roomRepository = new RoomRepository(_mockPartyRoomDbContext.Object);
        _rooms = new List<Room> { new Room { Id = 1, MaxOccupancy = 100, Attendees = 10 }, new Room { Id = 2, MaxOccupancy = 150, Attendees = 15 } };

        // === dbsetmock setup (uses AsyncEnumerator class defined at bottom also) ==
        var queryableRooms = _rooms.AsQueryable();
        _mockRoomsDbSet = new Mock<DbSet<Room>>();

        // === COMMON DBSET METHODS:
        /*
        _mockRoomsDbSet.Setup(m => m.FindAsync(someId)).Returns(ValueTask.FromResult(someObject));
        _mockRoomsDbSet.Setup(m => m.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>())).Callback<Room, CancellationToken>((room, _) => _rooms.Add(room));
        _mockRoomsDbSet.Setup(m => m.Update(It.IsAny<Room>())).Callback<Room>(room => room.MaxOccupancy = 999);
        _mockRoomsDbSet.Setup(m => m.Remove(It.IsAny<Room>())).Callback<Room>(room => _rooms.Remove(room));
        _mockRoomsDbSet.Setup(m => m.Add(It.IsAny<Room>())).Callback<Room>(room => _rooms.Add(room));
        _mockRoomsDbSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<Room>>())).Callback<IEnumerable<Room>>(rooms => _rooms.AddRange(rooms));
        _mockRoomsDbSet.Setup(m => m.Attach(It.IsAny<Room>())).Returns<EntityEntry<Room>>(room => room);
        _mockRoomsDbSet.Setup(m => m.Remove(It.IsAny<Room>())).Callback<Room>(room => _rooms.Remove(room));
        _mockRoomsDbSet.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<Room>>())).Callback<IEnumerable<Room>>(rooms => _rooms.RemoveAll(r => rooms.Contains(r)));
        _mockRoomsDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => _rooms.FirstOrDefault(r => r.Id == (int)ids[0]));
        _mockRoomsDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(ids => ValueTask.FromResult(_rooms.FirstOrDefault(r => r.Id == (int)ids[0])));
        _mockRoomsDbSet.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<Room>>(), CancellationToken.None)).Callback<IEnumerable<Room>, CancellationToken>((rooms, _) => _rooms.AddRange(rooms)).Returns(Task.CompletedTask);
        _mockRoomsDbSet.Setup(m => m.OrderByDescending(It.IsAny<Expression<Func<Room, object>>>())).Returns<Expression<Func<Room, object>>>(keySelector => _rooms.OrderByDescending(keySelector.Compile()).AsQueryable().OrderBy(_ => true) as IOrderedQueryable<Room>);
        */

        // === SETTING UP LINQ (sync)
        // Where(), Select(), GroupBy(), OrderBy(), ThenBy(), FirstOrDefault(),
        // SingleOrDefault(), ToList(), Any(), All(), Count(),
        // LongCount(), Sum(), Average(), Min(), Max()
        _mockRoomsDbSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(queryableRooms.Provider);
        _mockRoomsDbSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(queryableRooms.Expression);
        _mockRoomsDbSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(queryableRooms.ElementType);
        _mockRoomsDbSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(() => queryableRooms.GetEnumerator());

        // === SETTING UP LINQ (async)
        // ToListAsync(), AsAsyncEnumerable(),CountAsync(), LongCountAsync(),
        // FirstOrDefaultAsync(), SingleOrDefaultAsync(), AnyAsync(), AllAsync(),
        // SumAsync(), AverageAsync(), MinAsync(), MaxAsync(), GetAsyncEnumerator()
        _mockRoomsDbSet.As<IAsyncEnumerable<Room>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new AsyncEnumerator(queryableRooms.GetEnumerator()));
        
        _mockPartyRoomDbContext.Object.Rooms = _mockRoomsDbSet.Object;
        // === END of dbsetmock setup ==
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllPartyRooms()
    {
        // Arrange
        // LINQ GetAsyncEnumerator is setup in the setup method
        // This also takes care of ToListAsync(), AsAsyncEnumerable(),CountAsync(), LongCountAsync(),
        // FirstOrDefaultAsync(), SingleOrDefaultAsync(), AnyAsync(), AllAsync(), SumAsync(), AverageAsync(), MinAsync(), MaxAsync()

        // Act
        var result = await _roomRepository.GetAllAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Exactly(2).Items);
        Assert.That(result.Select(m => m.Id), Is.EquivalentTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsPartyRoomById()
    {
        // Arrange
        var roomId = 1;
        var room = _rooms.First(r => r.Id == roomId);
        _mockRoomsDbSet.Setup(m => m.FindAsync(roomId)).Returns(ValueTask.FromResult(room));

        // Act
        var result = await _roomRepository.GetByIdAsync(roomId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(roomId));
    }
    
    [Test]
    public async Task AddAsync_AddsNewPartyRoom()
    {
        // Arrange
        _mockRoomsDbSet.Setup(m => m.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>())).Callback<Room, CancellationToken>((room, _) => _rooms.Add(room));
        _mockPartyRoomDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1)); // 1 entity added/updated/deleted

        // Act
        await _roomRepository.AddAsync(new Room() { Id = 12, MaxOccupancy = 1222, Attendees = 122 }); 

        // Assert
        _mockRoomsDbSet.Verify(m => m.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockPartyRoomDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_UpdatesPartyRoom()
    {
        // Arrange
        _mockRoomsDbSet.Setup(m => m.Update(It.IsAny<Room>())).Callback<Room>(room => room.MaxOccupancy = 999);
        _mockPartyRoomDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1)); // 1 entity added/updated/removed
        
        // Act
        await _roomRepository.UpdateAsync(_rooms[0]);

        // Assert
        _mockRoomsDbSet.Verify(m => m.Update(It.IsAny<Room>()), Times.Once);
        _mockPartyRoomDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(_rooms[0].MaxOccupancy, Is.EqualTo(999));
    }

    [Test]
    public async Task DeleteAsync_DeletesPartyRoom()
    {
        // Arrange
        var roomWhichIsDeleted = _rooms[0];
        _mockRoomsDbSet.Setup(m => m.Remove(It.IsAny<Room>())).Callback<Room>(room => _rooms.Remove(room));

        // Act
        await _roomRepository.DeleteAsync(_rooms[0]);
            
        // Assert
        _mockRoomsDbSet.Verify(m => m.Remove(It.IsAny<Room>()), Times.Once);
        _mockPartyRoomDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(_rooms, Has.Count.EqualTo(1));
        Assert.That(_rooms, Has.None.EqualTo(roomWhichIsDeleted));
    }
}


// ========================================== INTERNAL CLASSES FOR ASYNC DBSET MOCKING ================================================

internal class AsyncEnumerator : IAsyncEnumerator<Room>
{
    private readonly IEnumerator<Room> _enumerator;
    public AsyncEnumerator(IEnumerator<Room> enumerator) { _enumerator = enumerator; }
    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
    public Room Current => _enumerator.Current;
    public ValueTask DisposeAsync() => default;
}