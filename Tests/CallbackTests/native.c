#define FUSE_USE_VERSION 31
#include <fuse.h>
#include <stddef.h>
#include <sys/stat.h>

size_t config_size(void) { return sizeof(struct fuse_config); }
size_t config_prefix_size(void)
{
    return offsetof(struct fuse_config, auto_cache) + sizeof(((struct fuse_config *)0)->auto_cache);
}

size_t config_offset(unsigned int index)
{
#define FIELD(name) offsetof(struct fuse_config, name)
    const size_t offsets[] = {
        FIELD(set_gid), FIELD(gid), FIELD(set_uid), FIELD(uid), FIELD(set_mode), FIELD(umask),
        FIELD(entry_timeout), FIELD(negative_timeout), FIELD(attr_timeout), FIELD(intr),
        FIELD(intr_signal), FIELD(remember), FIELD(hard_remove), FIELD(use_ino),
        FIELD(readdir_ino), FIELD(direct_io), FIELD(kernel_cache), FIELD(auto_cache)
    };
#undef FIELD
    return index < sizeof(offsets) / sizeof(offsets[0]) ? offsets[index] : (size_t)-1;
}

int invoke_rename(int (*callback)(const char *, const char *, unsigned int), unsigned int flags)
{
    return callback("/from", "/to", flags);
}

void *invoke_init(void *(*callback)(struct fuse_conn_info *, struct fuse_config *),
                  struct fuse_conn_info *conn, struct fuse_config *config)
{
    return callback(conn, config);
}

long utime_now(void) { return UTIME_NOW; }
long utime_omit(void) { return UTIME_OMIT; }
size_t timespec_size(void) { return sizeof(struct timespec); }
size_t timespec_offset(unsigned int index)
{
    const size_t offsets[] = { offsetof(struct timespec, tv_sec), offsetof(struct timespec, tv_nsec) };
    return index < sizeof(offsets) / sizeof(offsets[0]) ? offsets[index] : (size_t)-1;
}

int invoke_utimens(int (*callback)(const char *, const struct timespec[2], struct fuse_file_info *),
                   long atime_sec, long atime_nsec, long mtime_sec, long mtime_nsec,
                   struct fuse_file_info *info)
{
    const struct timespec times[2] = {
        { .tv_sec = atime_sec, .tv_nsec = atime_nsec },
        { .tv_sec = mtime_sec, .tv_nsec = mtime_nsec }
    };
    return callback("/file", times, info);
}
