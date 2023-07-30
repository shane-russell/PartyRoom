using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using PartyRoom.Data;
using PartyRoom.Domain;

namespace PartyRoom.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly PartyRoomDbContext _context;

        public RoomController(PartyRoomDbContext context)
        {
            _context = context;
        }

        // GET(read): api/Room
        [HttpGet]
        public async Task<IActionResult> GetRoomsAsync()
        {
            var rooms = await _context.Rooms.ToListAsync();
            
            return rooms.Count == 0 ? NoContent() : Ok(rooms);
        }

        // GET(read): api/Room
        [HttpGet]
        public IActionResult GetRooms()
        {
            var rooms = _context.Rooms.ToList();

            return rooms.Count == 0 ? NoContent() : Ok(rooms);
        }


        // GET(read): api/Room/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }


        // GET(read): api/Room/5
        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(int id)
        {
            var room = _context.Rooms.Find(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT(update): api/Room/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomAsync(int id, Room room)
        {
            if (room.Id != id)
            {
                return BadRequest("Room id mismatch");
            }

            Room roomToBeUpdated = _context.Rooms.Find(id);

            if (roomToBeUpdated == null)
            {
                return NotFound();
            }

            roomToBeUpdated.Id = room.Id;
            roomToBeUpdated.MaxOccupancy = room.MaxOccupancy;
            roomToBeUpdated.Attendees = room.Attendees;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT(update): api/Room/5
        [HttpPut("{id}")]
        public IActionResult PutRoom(int id, Room room)
        {
            if (room.Id != id)
            {
                return BadRequest("Room id mismatch");
            }

            Room roomToBeUpdated = _context.Rooms.Find(id);

            if (roomToBeUpdated == null)
            {
                return NotFound();
            }

            roomToBeUpdated.Id = room.Id;
            roomToBeUpdated.MaxOccupancy = room.MaxOccupancy;
            roomToBeUpdated.Attendees = room.Attendees;

            _context.SaveChanges();

            return NoContent();
        }

        // POST(create): api/Room
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        // POST(create): api/Room
        [HttpPost]
        public IActionResult PostRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();

            return Ok(room);
        }

        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
