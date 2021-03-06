﻿using Octokit;
using Octokit.Tests.Integration;
using Octokit.Tests.Integration.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

public class IssueCommentReactionsClientTests
{
    public class TheCreateReactionMethod : IDisposable
    {
        private readonly RepositoryContext _context;
        private readonly IIssuesClient _issuesClient;
        private readonly IGitHubClient _github;

        public TheCreateReactionMethod()
        {
            _github = Helper.GetAuthenticatedClient();
            var repoName = Helper.MakeNameWithTimestamp("public-repo");
            _issuesClient = _github.Issue;
            _context = _github.CreateRepositoryContext(new NewRepository(repoName)).Result;
        }

        [IntegrationTest]
        public async Task CanCreateReaction()
        {
            var newIssue = new NewIssue("a test issue") { Body = "A new unassigned issue" };
            var issue = await _issuesClient.Create(_context.RepositoryOwner, _context.RepositoryName, newIssue);

            Assert.NotNull(issue);

            var issueComment = await _issuesClient.Comment.Create(_context.RepositoryOwner, _context.RepositoryName, issue.Number, "A test comment");

            Assert.NotNull(issueComment);

            var issueCommentReaction = await _github.Reaction.IssueComment.Create(_context.RepositoryOwner, _context.RepositoryName, issueComment.Id, new NewReaction(ReactionType.Heart));

            Assert.NotNull(issueCommentReaction);

            Assert.IsType<Reaction>(issueCommentReaction);

            Assert.Equal(ReactionType.Heart, issueCommentReaction.Content);

            Assert.Equal(issueComment.User.Id, issueCommentReaction.User.Id);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
