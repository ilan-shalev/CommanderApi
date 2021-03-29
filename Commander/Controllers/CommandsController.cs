using Commander.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commander.Data;
using Commander.Dtos;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommanderRepo repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        [HttpGet] //GET api/commands
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            IEnumerable<Command> commands = _repo.GetAllCommands();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{id}", Name = nameof(GetCommandById))] // GET api/commands/{id}
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            Command command = _repo.GetCommandById(id);

            if (command != null)
                return Ok(_mapper.Map<CommandReadDto>(command));
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand([FromBody] CommandWriteDto writeDto) //...
        {
            Command cmd = _mapper.Map<Command>(writeDto);

            _repo.CreateCommand(cmd);

            _repo.SaveChanges();

            CommandReadDto readDto = _mapper.Map<CommandReadDto>(cmd);

            return CreatedAtRoute(nameof(GetCommandById), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandWriteDto writeDto)
        {
            Command cmdFromDb = _repo.GetCommandById(id);
            if (cmdFromDb == null)
                return NotFound();

            _mapper.Map(writeDto, cmdFromDb);

            _repo.UpdateCommand(cmdFromDb);

            _repo.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PatchCommand(int id,  JsonPatchDocument<CommandWriteDto> jsonPatch)
        {
            var commandFromRepo = _repo.GetCommandById(id);
            if (commandFromRepo == null)
                return NotFound();

            var commandToPatch = _mapper.Map<CommandWriteDto>(commandFromRepo);
            jsonPatch.ApplyTo(commandToPatch, ModelState);
            if (!TryValidateModel(ModelState))
                return ValidationProblem(ModelState);

            _mapper.Map(source: commandToPatch, destination: commandFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandFromDb = _repo.GetCommandById(id);
            if (commandFromDb == null)
                return NotFound();

            _repo.DeleteCommand(commandFromDb);
            _repo.SaveChanges();
            return NoContent();
        }
    }
}
