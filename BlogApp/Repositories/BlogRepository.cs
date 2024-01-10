using BlogApp.Data;
using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BlogApp.ViewModels;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.UI.WebControls;

namespace BlogApp.Repositories
{
    public class BlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }

        public List<Post> GetPosts()
        {
            return _context.Posts.Include(p => p.Comments).ToList();
        }

        /* public PostViewModel GetPostById(int postId)
         {
             var user = (User)HttpContext.Current.Session["AuthenticatedUser"];

             var postViewModel = _context.Posts
                 .Include(p => p.user)
                 .Include(p => p.Comments.Select(c => c.commentAttachments))  
                 .Include(p => p.Attachments)
                 .Include(p => p.Likes)
                 .AsEnumerable()
                 .Select(p => new PostViewModel
                 {
                     PostId = p.PostId,
                     Title = p.Title,
                     Content = p.Content,
                     CreatedAt = p.CreatedAt,
                     UserId = p.UserId,
                     UserName = p.user.FirstName + p.user.LastName,
                     Attachments = p.Attachments,
                     Likes = p.Likes,
                     UserHasLiked = p.Likes.Any(l => l.UserId == user.UserId && l.PostId == p.PostId),
                     UserIdOfUserAccessingPost = user.UserId,
                     // Map comments for the post along with CommentAttachments
                     Comments = p.Comments.Select(c => new CommentViewModel
                     {
                         CommentId = c.CommentId,
                         CommentText = c.Content,
                         CreatedAt = c.CreatedAt,
                         UserName = _context.Users
                         .Where(u => u.UserId == c.UserId)
                         .Select(u => u.FirstName + " " + u.LastName)
                         .FirstOrDefault(),
                         UserHasLiked = c.Likes.Any(l => l.UserId == user.UserId && l.commentId == c.CommentId),
                         Likes = c.Likes,
                         // Map CommentAttachments for each Comment
                         commentAttachments = c.commentAttachments.Select(ca => new CommentAttachment
                         {
                             CommentAttachmentId = ca.CommentAttachmentId,
                             FileName = ca.FileName,
                             FilePath = ca.FilePath
                         }).ToList()

                     }).ToList()

                 })
                 .FirstOrDefault(p => p.PostId == postId);

             return postViewModel;
         } */

        public PostViewModel GetPostById(int postId)
        {
            var user = (User)HttpContext.Current.Session["AuthenticatedUser"];

            var postViewModel = _context.Posts
                .Include(p => p.user)
                .Include(p => p.Comments.Select(c => c.commentAttachments))
                .Include(p => p.Attachments)
                .Include(p => p.Likes)
                .AsEnumerable()
                .Select(p => MapPostDetails(p, user)) //Add Post detail to view model. 
                .FirstOrDefault(p => p.PostId == postId);

            return postViewModel;
        }

        private PostViewModel MapPostDetails(Post post, User user)
        {
            return new PostViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                UserName = post.user.FirstName + post.user.LastName,
                Attachments = post.Attachments,
                Likes = post.Likes,
                UserHasLiked = post.Likes.Any(l => l.UserId == user.UserId && l.PostId == post.PostId),
                UserIdOfUserAccessingPost = user.UserId,
                Comments = MapComments(post.Comments, user)
            };
        }

        private List<CommentViewModel> MapComments(IEnumerable<Comment> comments, User user)
        {
            return comments
            .Where(c => c.ParentCommentId == null && (c._CommentStatus == CommentStatus.Live || c._CommentStatus == CommentStatus.Reported))
            .Select(c => new CommentViewModel
            {
                CommentId = c.CommentId,
                CommentText = c.Content,
                CreatedAt = c.CreatedAt,
                UserName = _context.Users
                    .Where(u => u.UserId == c.UserId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault(),
                UserHasLiked = c.Likes.Any(l => l.UserId == user.UserId && l.commentId == c.CommentId),
                Likes = c.Likes,
                commentAttachments = MapCommentAttachments(c.commentAttachments),
                ChildComments = MapChildComments(comments, user, c.CommentId),

            }).ToList();
        }

        private List<CommentViewModel> MapChildComments(IEnumerable<Comment> comments, User user, int parentId)
        {
            return comments
                .Where(c => c.ParentCommentId == parentId && (c._CommentStatus == CommentStatus.Live || c._CommentStatus == CommentStatus.Reported))
                .Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    CommentText = c.Content,
                    CreatedAt = c.CreatedAt,
                    PostId = c.PostId,
                    UserName = _context.Users
                        .Where(u => u.UserId == c.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),
                    UserHasLiked = c.Likes.Any(l => l.UserId == user.UserId && l.commentId == c.CommentId),
                    Likes = c.Likes,
                    commentAttachments = MapCommentAttachments(c.commentAttachments),
                    // Recursively fetch and map child comments
                    ChildComments = MapChildComments(comments, user, c.CommentId)
                }).ToList();
        }


        private List<CommentAttachment> MapCommentAttachments(IEnumerable<CommentAttachment> attachments)
        {
            return attachments.Select(ca => new CommentAttachment
            {
                CommentAttachmentId = ca.CommentAttachmentId,
                FileName = ca.FileName,
                FilePath = ca.FilePath
            }).ToList();
        }


        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public List<PostViewModel> GetAllPosts()
        {

            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Where(p => p.PostStatus == Status.Live || p.PostStatus == Status.Reported)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.user.FirstName + p.user.LastName,

            })
                .ToList();

            return postViewModels;

        }

        public List<PostViewModel> GetAllPendingPosts()
        {

            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Where(p => p.PostStatus == Status.PendingApproval)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.user.FirstName + p.user.LastName,
            })
                .ToList();

            return postViewModels;

        }

        public List<PostViewModel> GetAllRepoetedPost()
        {
            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Where(p => p.PostStatus == Status.Reported)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.user.FirstName + p.user.LastName,
            })
                .ToList();

            return postViewModels;
        }
        public void AddLike(int userId, int postId)
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.Now,
            };

            _context.Likes.Add(like);
            _context.SaveChanges();
        }

        public bool HasUserLikedPost(int userId, int postId)
        {
            var hasLiked = _context.Likes.Any(l => l.PostId == postId && l.UserId == userId);
            return hasLiked;
        }

        public void UnlikePost(int userId, int postId)
        {
            var likeToRemove = _context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (likeToRemove != null)
            {
                _context.Likes.Remove(likeToRemove);
                _context.SaveChanges();
            }
        }

        public void AddComment(int userId,int postId ,string commentText, int? parentCommentId = null)
        {
            var newComment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = commentText,
                CreatedAt = DateTime.Now,
                _CommentStatus = CommentStatus.Live,
                ParentCommentId = parentCommentId,
            };

            _context.Comments.Add(newComment);

            _context.SaveChanges();
        }

        public void AddCommentWithAttachment(int userId, int postId, string commentText, string attachmentUrl, string FileName, int? parentCommentId = null)
        {
            var newComment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = commentText,
                CreatedAt = DateTime.Now,
                _CommentStatus = CommentStatus.Live,
                ParentCommentId = parentCommentId,
            };

            _context.Comments.Add(newComment);

            int commentId = newComment.CommentId;

            var newCommentAttachment = new CommentAttachment
            {
                FilePath = attachmentUrl,
                FileName = FileName,
                CommentId = commentId,
            };
            _context.CommentsAttachment.Add(newCommentAttachment);

            _context.SaveChanges();
        }

        //Get New Post For DashBoard
        public List<PostViewModel> GetMostRecentPosts(int count)
        {
            var posts = _context.Posts
                .Include(p => p.user)
                .OrderByDescending(p => p.CreatedAt)
                .Where(p => p.PostStatus == Status.Live || p.PostStatus == Status.Reported)
                .Take(count)
                .Select(p => new PostViewModel
                {
                    //Map Properties with Post View Model Entities
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserName = p.user.FirstName + " " + p.user.LastName,
                })
                .ToList();

            return posts;
        }

        //Get moser Recent Comments
        public List<CommentViewModel> GetMostRecentComments(int count)
        {
            var comments = _context.Comments
            .Where(l=> (l._CommentStatus == CommentStatus.Live) || (l._CommentStatus == CommentStatus.Reported))
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            //.Include(c => c.PostId)
            .Select(c => new CommentViewModel
            {
                CommentId = c.CommentId,
                CommentText = c.Content,
                CreatedAt = c.CreatedAt,
                PostId = c.PostId,
                UserId = c.UserId,
                UserName = _context.Users 
                    .Where(u => u.UserId == c.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),
                PostTitle = _context.Posts.Where(p => p.PostId  == c.PostId).Select(p => p.Title).FirstOrDefault(),
            })
            .ToList();
            return comments;
        }

        public List<LikeViewModel> GetMostRecentLikes(int count)
        {
            var likes = _context.Likes
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .Select(l => new LikeViewModel
                {

                    UserName = _context.Users.Where(u => u.UserId == l.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),

                    PostTitle = _context.Posts.Where(p => p.PostId == l.PostId).Select(p => p.Title).FirstOrDefault(),
                    CreatedAt = l.CreatedAt,
                    PostId = l.PostId
                })
                .ToList();

            return likes;
        }

        public void ApprovePost(int postId)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.Live;

                // Save changes to the database
                _context.SaveChanges();
            }
        }

        public void RejectPost(int postId)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.Rejected;
                _context.SaveChanges();
            }
        }

        public void AcceptReportedComment(int commentId)
        {
            var comment = _context.Comments.Find(commentId);

            if (comment != null)
            {
                comment._CommentStatus = CommentStatus.DeletedAfterReport;

                var reportedComments = _context.ReportComments.Where(rc => rc.CommenntId == commentId);
                _context.ReportComments.RemoveRange(reportedComments);

                _context.SaveChanges();
            }
        }

        public void RejectReportedComment(int commentId)
        {
            var comment = _context.Comments.Find(commentId);

            if (comment != null)
            {
                comment._CommentStatus = CommentStatus.Live;

                var reportedComments = _context.ReportComments.Where(rc => rc.CommenntId == commentId);
                _context.ReportComments.RemoveRange(reportedComments);

                _context.SaveChanges();
            }
        }

        public void ReportPost( int postId, int userIdWhoReport, string reportReason)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                // Create a new ReportPost instance
                var reportPost = new ReportPost
                {
                    PostId = postId,
                    UserId = userIdWhoReport,
                    ReportReason = reportReason
                };

                // Add the new report to the context and save changes
                _context.ReportPosts.Add(reportPost);
                _context.SaveChanges();
            }

        }

        public void AddSuggestion(int postId, int idWhoMadeSuggestion, string suggestion)
        {
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                var reportPost = new PostSuggestion
                {
                    PostId = postId,
                    UserId = idWhoMadeSuggestion,
                    suggestionStatus = SuggestionStatus.Live,
                    SuggestionText = suggestion
                };

                // Add the new report to the context and save changes
                _context.PostSuggestions.Add(reportPost);
                _context.SaveChanges();
            }

        }

        public List<ReportPostViewModel> GetReportedDetails()
        {

            var reportedPosts = _context.ReportPosts
            .ToList();


            //Make an List of ReportPostViewModel
            var reportPostViewModels = reportedPosts.Select(rp => new ReportPostViewModel
            {
                Post = GetPostById(rp.PostId),
                PostId = rp.PostId,
                UserId = rp.UserId,
                Reason = rp.ReportReason,
                RepoeterName = _context.Users.Where(u => u.UserId == rp.UserId).Select(u => u.FirstName).FirstOrDefault()
            }).ToList();

            return reportPostViewModels;
        }

        public List<ReportCommentViewModel> GetReportedComments()
        {
            var reportedComments = _context.ReportComments
              .Join(
                  _context.Comments,
                  rc => rc.CommenntId,
                  c => c.CommentId,
                  (rc, c) => new { ReportComment = rc, Comment = c }
              )
              .ToList();

            var reportCommentViewModels = reportedComments.Select(rc => new ReportCommentViewModel
            {
                Comment = new CommentViewModel
                {
                    CommentId = rc.Comment.CommentId,
                    CommentText = rc.Comment.Content, 
                                                      
                },
                UserId = rc.ReportComment.UserId,
                CommentId = rc.ReportComment.CommenntId,
                Reason = rc.ReportComment.ReportReason,
                RepoeterName = _context.Users
                    .Where(u => u.UserId == rc.ReportComment.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault()
            }).ToList();

            return reportCommentViewModels;

        }



        public void DeleteReportedPost(int postId)
        {
            var reportedPosts = _context.ReportPosts.Where(rp => rp.PostId == postId).ToList();

            foreach (var reportedPost in reportedPosts)
            {
                // Remove each reported post entry
                _context.ReportPosts.Remove(reportedPost);
            }

            // Save changes to the database
            _context.SaveChanges();
        }

        public void AcceptReportToPost(int postId)
        {
            // Implement logic to update the post status or take necessary actions
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.DeletedAfterReport; 

                _context.SaveChanges();
                DeleteReportedPost(postId);
            }
            else
            {
                throw new InvalidOperationException("Post not found.");
            }
        }

        public void RejectReportToPost(int postId)
        {
            // Implement logic to update the post status or take necessary actions
            var post = _context.Posts.Find(postId);

            if (post != null)
            {
                post.PostStatus = Status.Live;
                
                _context.SaveChanges();
                DeleteReportedPost(postId);

            }
            else
            {
                throw new InvalidOperationException("Post not found.");
            }
        }

        public bool HasUserLikedComment(int userId, int commentId)
        {
            var hasLiked = _context.CommentLikes.Any(l => l.commentId == commentId && l.UserId == userId);
            return hasLiked;
        }

        public void AddLikeToComment(int commentId, int userId)
        {
            if (!HasUserLikedComment(userId, commentId))
            {
                var commentLike = new CommentLike
                {
                    commentId = commentId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.CommentLikes.Add(commentLike);
                _context.SaveChanges();
            }

        }
        public void RemoveLikeToComment(int commentId, int userId)
        {
            var likeToRemove = _context.CommentLikes.FirstOrDefault(l => l.commentId == commentId && l.UserId == userId);

            if (likeToRemove != null)
            {
                _context.CommentLikes.Remove(likeToRemove);
                _context.SaveChanges();
            }

        }

        public CommentViewModel GetCommentById(int commentId)
        {
            var comment = _context.Comments
                .Where(c => c.CommentId == commentId)
                .Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    CommentText = c.Content,
                    UserId = c.UserId,
                    PostId = c.PostId,
                    CreatedAt = c.CreatedAt,
                    UserName = _context.Users.Where(l => c.UserId == l.UserId).Select(u => u.FirstName).FirstOrDefault(),
                })
                .FirstOrDefault();

            return comment;
        }

        public void ReportComment(int commentId, int userIdWhoReport, string reportReason)
        {
            var post = _context.Comments.Find(commentId);

            if (post != null)
            {
                // Create a new ReportComment instance
                var reportComment = new ReportComment
                {
                    CommenntId = commentId,
                    UserId = userIdWhoReport,
                    ReportReason = reportReason,
                    ReportedAt = DateTime.Now,
                };

                // Add the new report to the context and save changes
                _context.ReportComments.Add(reportComment);
                _context.SaveChanges();
            }

        }

        public List<SuggetionPageViewModel> GetSuggestionsGivenByUser(int userId)
        {
            List<SuggetionPageViewModel> suggetionPageViewModels = _context.PostSuggestions
            .Where(s => s.UserId == userId)
            .Select(s => new SuggetionPageViewModel
            {
                Id = s.Id,
                PostId = s.PostId,
                SuggestionText = s.SuggestionText,
                suggestionStatus = s.suggestionStatus,
                Post = new PostViewModel
                {
                    PostId = s.PostId,
                    Title = _context.Posts.Where(p => p.PostId == s.PostId).Select(p => p.Title).FirstOrDefault(),
                    Content = _context.Posts.Where(p => p.PostId == s.PostId).Select(p => p.Content).FirstOrDefault()
                }
            }
            )
            .ToList();

            return suggetionPageViewModels;

        }

        public SuggetionPageViewModel GetSuggestionById(int suggestionId)
        {
            var suggestion = _context.PostSuggestions
                .Where(s => s.Id == suggestionId)
                .Select(s => new SuggetionPageViewModel
                {
                    Id = s.Id,
                    PostId = s.PostId,
                    SuggestionText = s.SuggestionText,
                    suggestionStatus = s.suggestionStatus,
                    Post = new PostViewModel
                    {
                        PostId = s.PostId,
                        Title = _context.Posts.Where(p => p.PostId == s.PostId).Select(p => p.Title).FirstOrDefault(),
                        Content = _context.Posts.Where(p => p.PostId == s.PostId).Select(p => p.Content).FirstOrDefault(),
                        CreatedAt = _context.Posts.Where(p => p.PostId == s.PostId).Select(p => p.CreatedAt).FirstOrDefault()
                    },
                    UserName = _context.Users
                        .Where(u => u.UserId == s.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),
                })
                .FirstOrDefault();

            return suggestion;
        }

        public List<SuggetionPageViewModel> GetSuggestionsReceivedByUser(int userId)
        {
            List<SuggetionPageViewModel> suggestions = _context.PostSuggestions
                .Join(
                    _context.Posts.Where(post => post.UserId == userId),
                    suggestion => suggestion.PostId,
                    post => post.PostId,
                    (suggestion, post) => new SuggetionPageViewModel
                    {
                        Id = suggestion.Id,
                        SuggestionText = suggestion.SuggestionText,
                        suggestionStatus = suggestion.suggestionStatus,
                        Post = new PostViewModel
                        {
                            PostId = post.PostId,
                            UserIdOfUserAccessingPost = userId,
                            Title = post.Title,
                            CreatedAt = post.CreatedAt,
                        },
                        UserName = _context.Users
                        .Where(u => u.UserId == suggestion.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),
                    }
                 ).ToList();
            
            return suggestions;
        }

        public void RejectSuggestion(int suggestionId)
        {
            var suggestion = _context.PostSuggestions.Find(suggestionId);

            if (suggestion != null)
            {
                suggestion.suggestionStatus = SuggestionStatus.Rejected;
                _context.SaveChanges();
            }
        }

        public List<PostViewModel> GetPostsByUserId(int userId)
        {
            var posts = _context.Posts
                .Where(p => p.UserId == userId && (p.PostStatus == Status.Live || p.PostStatus == Status.Reported))
                .Select(p => new PostViewModel
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserName = p.user.FirstName + " " + p.user.LastName,
                })
                .ToList();
            return posts;
        }

        public void DeactivatePost(int PostId)
        {
            var post = _context.Posts.Find(PostId);
            if (post != null)
            {
                post.PostStatus = Status.Deleted;
                _context.SaveChanges();
            }
        }

        public void UpdatePost(PostViewModel updatedPost)
        {
            var existingPost = _context.Posts.Find(updatedPost.PostId);
            if(existingPost != null)
            {
                existingPost.Title = updatedPost.Title;
                existingPost.Content = updatedPost.Content;
                _context.SaveChanges();
            }
        }

        public void UpdateSuggestionStatus(int SuggestionId)
        {
            var suggestion = _context.PostSuggestions.Find(SuggestionId);

            if (suggestion != null)
            {
                suggestion.suggestionStatus = SuggestionStatus.Accepted;
                _context.SaveChanges();
            }
        }
    }
}
