using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Interfaces;
using Service.Interfaces.Storage;
using System;

namespace Service.Link
{
    public abstract class BaseCommand<TPayload, TArgument>
    {
        protected readonly ILinkRepository _linkRepository;
        protected readonly IStorage _storageService;

        protected BaseCommand(ILinkRepository linkRepository, IStorage storageService)
        {
            _linkRepository = linkRepository;
            _storageService = storageService;
        }

        protected TPayload Execute(MediaType type, TArgument argument, params object[] args)
        {
            switch (type)
            {
                case MediaType.Music: return ExecuteMusic(argument, args);
                case MediaType.Ticket: return ExecuteTicket(argument, args);
                default: throw new NotSupportedException($"Link type {type} is not supported.");
            }
        }

        protected abstract TPayload ExecuteMusic(TArgument argument, params object[] args);

        protected abstract TPayload ExecuteTicket(TArgument argument, params object[] args);
    }

    public abstract class BaseCommand<TArgument>
    {
        protected readonly ILinkRepository _linkRepository;
        protected readonly IStorage _storageService;

        protected BaseCommand(ILinkRepository linkRepository, IStorage storageService)
        {
            _linkRepository = linkRepository;
            _storageService = storageService;
        }

        protected void Execute(MediaType type, TArgument argument, params object[] args)
        {
            switch (type)
            {
                case MediaType.Music: ExecuteMusic(argument, args); break;
                case MediaType.Ticket: ExecuteTicket(argument, args); break;
                default: throw new NotSupportedException($"Link type {type} is not supported.");
            }
        }

        protected abstract void ExecuteMusic(TArgument argument, params object[] args);

        protected abstract void ExecuteTicket(TArgument argument, params object[] args);
    }
}