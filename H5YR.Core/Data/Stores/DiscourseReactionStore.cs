using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace H5YR.Core.Data.Stores
{
    public class DiscourseReactionStore : IDiscourseReactionStore
    {
        private readonly ILogger<DiscourseReactionStore> _logger;
        private readonly IScopeProvider _scopeProvider;

        public DiscourseReactionStore(ILogger<DiscourseReactionStore> logger, IScopeProvider scopeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public IEnumerable<DiscourseReaction> GetAll()
        {
            using var scope = _scopeProvider.CreateScope();
            var reactions = scope.Database.Fetch<DiscourseReaction>();
            scope.Complete();
            return reactions;
        }

        public IEnumerable<DiscourseReaction> GetRecent(int count)
        {
            using var scope = _scopeProvider.CreateScope();
            var sql = scope.SqlContext.Sql()
                .Select("*")
                .From<DiscourseReaction>()
                .OrderByDescending<DiscourseReaction>(x => x.CreatedAt);

            var reactions = scope.Database.Fetch<DiscourseReaction>(sql);
            scope.Complete();
            return reactions;
        }

        public DiscourseReaction? GetByPostId(int postId)
        {
            using var scope = _scopeProvider.CreateScope();
            var reaction = scope.Database.FirstOrDefault<DiscourseReaction>("WHERE PostId = @0", postId);
            scope.Complete();
            return reaction;
        }

        public bool ReactionExists(int postId)
        {
            using var scope = _scopeProvider.CreateScope();
            var exists = scope.Database.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM DiscourseReactions WHERE PostId = @0", postId) > 0;
            scope.Complete();
            return exists;
        }

        public void Save(DiscourseReaction reaction)
        {
            using var scope = _scopeProvider.CreateScope();
            scope.Database.Save(reaction);
            scope.Complete();
            _logger.LogInformation("Saved Discourse reaction for post {PostId}", reaction.PostId);
        }

        public void Delete(DiscourseReaction reaction)
        {
            using var scope = _scopeProvider.CreateScope();
            scope.Database.Delete(reaction);
            scope.Complete();
        }

        public int GetTotalCount()
        {
            using var scope = _scopeProvider.CreateScope();
            var count = scope.Database.ExecuteScalar<int>("SELECT COUNT(*) FROM DiscourseReactions");
            scope.Complete();
            return count;
        }
    }
}