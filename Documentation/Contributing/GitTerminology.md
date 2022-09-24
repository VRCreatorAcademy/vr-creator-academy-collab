# Git Terminology
If you think you hear a some git terminology that is not listed here open an issue.  If the terminology it significantly 
advanced in nature then this simple glossary is no substitution for the 
[official git documentation](https://git-scm.com/doc).  

[If all else fails then click this link.](https://lmgtfy.app/?q=git+terminology)

## branch
A branch is a point in time (a commit) where a project deviates.  This deviation is often the result of multiple people
working on different features at the same time from a common starting point.  Once the features are complete branches
can be merged back into the "main" branch (often called the trunk in other scm software)
## clone
The process of copying repository from a remote directory to your local directory.
## fork
A fork is a copy of a repository.  The term is reused and has different meanings depending on where it is use.  This
can cause a bit of confusion.
### fork (in github.com terms)
A fork is a copy of a repository in your GitHub account that is tied to another repository on github.  A github fork
doen not have all the features of a repository that has been created on your account.  Specifically you cannot have
issues specific to your fork, all the issues are maintained on the upstream repository.
### fork (in open source terms)
A fork is a copy of a repository that is made because of a difference of opinion on the direction that a project should
go.  When the difference of opinion occur an individual or group may leave decide to copy the repositior and take the
project in a different direction.  Forking a project is generally not considered a good thing as it dilutes resources
from a project because the team becomes split.
## git vs github.com vs git client
- git: is a command line tool that is the underlying software that manages the repository
- github.com: is a website that allows users to store git repositories on a globally accessible server (the remote)
- git client: A program that wraps the git command line interface in graphical user interface to make using git "easier" 
## local (repository)
The repository you are currently working in.
## repository or repo
A store of data for a project that is often version controlled. A clone of a git repository usually contain every commit
ever made by every contributor on your local copy.  This has the advantage of not needing an internet connection to see
what changes were made over time.  This allows you to get on an airplane or a cruise ship and review changes as well as
make changes that can then be later pushed to a remote.
## remote (repository)
The remote is another repository other than the one you are currently working on. It is usually another computer that
is accessible on the internet but it doesn't have to be.  It can be another computer on your network or even another
directory on your own computer.
## source control management (appreciated scm)
A set of tool that help manage different versions control.  Git is a source control management tool.
## upstream
A repository that is the parent project that owners forks contribute changes back to. 
## working directory
A copy of the project at a given commit that you can modify.
### dirty (working directory)
Your working directory is "dirty" when you have altered in some way from the commit.
### clean (working directory)
Your working directory is "clean" when it is unaltered altered the commit stored in the repository.

