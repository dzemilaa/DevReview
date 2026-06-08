using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using MediatR;

namespace DevReview.Application.Tags
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, IReadOnlyList<TagDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTagsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _unitOfWork.Tags.GetAllAsync(cancellationToken);
            return tags.Select(_mapper.Map<TagDto>).ToList();
        }
    }

    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.Tags.FindAsync(x => x.Name.ToLower() == request.Name.Trim().ToLower(), cancellationToken);
            if (existing.Any())
            {
                throw new ApplicationException("A tag with the same name already exists.");
            }

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim()
            };

            await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TagDto>(tag);
        }
    }

    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, TagDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TagDto> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
            if (tag == null)
            {
                throw new ApplicationException("Tag not found.");
            }

            var conflict = await _unitOfWork.Tags.FindAsync(x => x.Name.ToLower() == request.Name.Trim().ToLower() && x.Id != request.Id, cancellationToken);
            if (conflict.Any())
            {
                throw new ApplicationException("A tag with the same name already exists.");
            }

            tag.Name = request.Name.Trim();
            _unitOfWork.Tags.Update(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TagDto>(tag);
        }
    }

    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTagCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
            if (tag == null)
            {
                throw new ApplicationException("Tag not found.");
            }

            _unitOfWork.Tags.Remove(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
