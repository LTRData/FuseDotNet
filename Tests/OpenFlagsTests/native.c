#define _GNU_SOURCE
#define FUSE_USE_VERSION 31
#include <fuse.h>
#include <fcntl.h>
#include <stddef.h>

int open_flag_value(unsigned int index)
{
    const int values[] = { O_RDONLY, O_WRONLY, O_RDWR, O_ACCMODE, O_NONBLOCK,
        O_APPEND, O_ASYNC, O_SYNC, O_NOFOLLOW, O_CREAT, O_TRUNC, O_EXCL,
        O_NOCTTY, O_DIRECT, O_DIRECTORY, O_CLOEXEC, O_DSYNC,
#ifdef __FreeBSD__
        O_SHLOCK, O_EXLOCK, O_EXEC, O_SEARCH, O_TTY_INIT, O_VERIFY, O_RESOLVE_BENEATH
#else
        -1, -1, -1, -1, -1, -1, -1
#endif
    };
    return index < sizeof(values) / sizeof(values[0]) ? values[index] : -1;
}

void set_flags(struct fuse_file_info *info, int flags) { info->flags = flags; }
int get_flags(const struct fuse_file_info *info) { return info->flags; }
size_t flags_size(void) { return sizeof(((struct fuse_file_info *)0)->flags); }
size_t file_info_offset(unsigned int index)
{
    const size_t offsets[] = { offsetof(struct fuse_file_info, flags), offsetof(struct fuse_file_info, fh),
        offsetof(struct fuse_file_info, lock_owner), offsetof(struct fuse_file_info, poll_events) };
    return index < sizeof(offsets) / sizeof(offsets[0]) ? offsets[index] : (size_t)-1;
}
