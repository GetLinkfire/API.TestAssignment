using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using Service.Interfaces.Commands;
using Service.Links;
using Service.Models.Link;
using Service.Models.Link.Music;
using Service.Models.Link.Ticket;
using WebApp.Mappings;
using WebApp.Models;

namespace WebApp.Controllers
{
	[RoutePrefix("links")]
	public class LinkController : ApiController
	{
		private readonly ICommand<LinkModel, CreateLink> _createCommand;
		private readonly ICommand<ExtendedLinkModel, UpdateLink> _updateCommand;
		private readonly ICommand<ExtendedLinkModel, GetLink> _getCommand;
		private readonly ICommand<DeleteLink> _deleteCommand;

		public LinkController(
			ICommand<LinkModel, CreateLink> createCommand,
			ICommand<ExtendedLinkModel, UpdateLink> updateCommand,
			ICommand<ExtendedLinkModel, GetLink> getCommand,
			ICommand<DeleteLink> deleteCommand)
		{
			_getCommand = getCommand;
			_createCommand = createCommand;
			_updateCommand = updateCommand;
			_deleteCommand = deleteCommand;
		}

		[HttpGet]
		[Route("{linkId:guid}")]
		[ResponseType(typeof(LinkDto))]
		public HttpResponseMessage Get([FromUri] Guid linkId)
		{
			var result = _getCommand.Execute(new GetLink() { LinkId = linkId });
			var mapped = Mapper.Map<LinkDto>(result);

			mapped.MusicDestinations = result.MusicDestinations?.ToDestinationDto<MusicDestinationModel, MusicDestinationDto>();
			mapped.TicketDestinations = result.TicketDestinations?.ToDestinationDto<TicketDestinationModel, TicketDestinationDto>();

			return Request.CreateResponse(HttpStatusCode.OK, mapped);
		}

		[HttpPost]
		[Route("")]
		[ResponseType(typeof(LinkDto))]
		public HttpResponseMessage Create([FromBody] CreateLinkDto link)
		{
			var request = Mapper.Map<CreateLink>(link);
			var result = _createCommand.Execute(request);

			var viewModel = Mapper.Map<LinkDto>(result);

			return Request.CreateResponse(HttpStatusCode.OK, viewModel);
		}

		[HttpPut]
		[Route("{linkId:guid}")]
		[ResponseType(typeof(LinkDto))]
		public HttpResponseMessage Update([FromUri] Guid linkId, [FromBody] LinkDto link)
		{
			link.Id = linkId;
			var request = Mapper.Map<UpdateLink>(link);
			var result = _updateCommand.Execute(request); 

			var viewModel = Mapper.Map<LinkDto>(result);
			viewModel.MusicDestinations = result.MusicDestinations?.ToDestinationDto<MusicDestinationModel, MusicDestinationDto>();
			viewModel.TicketDestinations = result.TicketDestinations?.ToDestinationDto<TicketDestinationModel, TicketDestinationDto>();

			return Request.CreateResponse(HttpStatusCode.OK, viewModel);
		}

		[HttpDelete]
		[Route("{linkId:guid}")]
		public HttpResponseMessage Delete([FromUri] Guid linkId)
		{
			_deleteCommand.Execute(new DeleteLink() { LinkId = linkId });
			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}
