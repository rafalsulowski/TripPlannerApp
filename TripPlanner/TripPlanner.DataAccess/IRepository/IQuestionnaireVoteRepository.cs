﻿using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IQuestionnaireVoteRepository : IRepository<QuestionnaireVote>
    {
        Task<RepositoryResponse<bool>> Update(QuestionnaireVote post);
    }
}
