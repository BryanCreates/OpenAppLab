using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Mod.Posts.Server;
public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}
