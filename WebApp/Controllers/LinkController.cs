using AutoMapper;
using Service.Interfaces.Commands;
using Service.Link;
using Service.Models.Link;
using System;
using System.Data.Entity.Core;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Controllers
{
    [RoutePrefix("links")]
	public class LinkController : ApiController
	{
		private readonly ICommand<LinkModel, CreateLinkArgument> _createCommand;
		private readonly ICommand<ExtendedLinkModel, UpdateLinkArgument> _updateCommand;
		private readonly ICommand<ExtendedLinkModel, GetLinkArgument> _getCommand;
		private readonly ICommand<DeleteLinkArgument> _deleteCommand;

		public LinkController(
			ICommand<LinkModel, CreateLinkArgument> createCommand,
			ICommand<ExtendedLinkModel, UpdateLinkArgument> updateCommand,
			ICommand<ExtendedLinkModel, GetLinkArgument> getCommand,
			ICommand<DeleteLinkArgument> deleteCommand)
		{
			_createCommand = createCommand;
			_updateCommand = updateCommand;
			_getCommand = getCommand;
			_deleteCommand = deleteCommand;
		}

		[HttpGet]
		[Route("{linkId:guid}")]
		[ResponseType(typeof(LinkDto))]
		public HttpResponseMessage Get([FromUri] Guid linkId)
		{
			ExtendedLinkModel result = _getCommand.Execute(new GetLinkArgument() { LinkId = linkId });

			LinkDto mapped = Mapper.Map<LinkDto>(result);

			mapped.MusicDestinations = result.MusicDestinations?.ToDto();

			mapped.TicketDestinations = result.TicketDestinations?.ToDto();

			return Request.CreateResponse(HttpStatusCode.OK, mapped);
		}

		[HttpPost]
		[Route("")]
		[ResponseType(typeof(LinkDto))]
		public HttpResponseMessage Create([FromBody] CreateLinkDto link)
		{
			var result = _createCommand.Execute(new CreateLinkArgument()
			{
				Link = Mapper.Map<LinkModel>(link),
				MusicDestinations = link.MusicDestinations?.ToModel(),
				TicketDestinations = link.TicketDestinations?.ToModel()
			});

			LinkDto mapped = Mapper.Map<LinkDto>(result);
			
			return Request.CreateResponse(HttpStatusCode.OK, mapped);
		}

		[HttpPut]
		[Route("{linkId:guid}")]
		[ResponseType(typeof(LinkDto))]
		public HttpResponseMessage Update([FromUri] Guid linkId, [FromBody] LinkDto link)
		{
			link.Id = linkId;

			UpdateLinkArgument argument = new UpdateLinkArgument()
			{
				Link = Mapper.Map<ExtendedLinkModel>(link)
			};

			argument.Link.MusicDestinations = link.MusicDestinations?.ToModel();

			argument.Link.TicketDestinations = link.TicketDestinations?.ToModel();
			
			ExtendedLinkModel result = _updateCommand.Execute(argument);

			LinkDto mapped = Mapper.Map<LinkDto>(result);

			mapped.MusicDestinations = result.MusicDestinations?.ToDto();

			mapped.TicketDestinations = result.TicketDestinations?.ToDto();
			
			return Request.CreateResponse(HttpStatusCode.OK, mapped);
		}

		[HttpDelete]
		[Route("{linkId:guid}")]
		public HttpResponseMessage Delete([FromUri] Guid linkId)
		{
			_deleteCommand.Execute(new DeleteLinkArgument() { LinkId = linkId });

			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}
