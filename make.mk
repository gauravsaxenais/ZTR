# ╔═══════════════════════════════════════════════════════════════════════════╗
# ║                            Project Attributes                             ║
# ╚═══════════════════════════════════════════════════════════════════════════╝

PROJECT_NAME := back-end

# ╔═══════════════════════════════════════════════════════════════════════════╗
# ║                                  Version                                  ║
# ╚═══════════════════════════════════════════════════════════════════════════╝

# Determine the version of the image, based on CI tag, or dev for local builds.
ifeq ($(CI_COMMIT_TAG),)
   VERSION=dev
else
   VERSION=$(CI_COMMIT_TAG)
endif

# ╔═══════════════════════════════════════════════════════════════════════════╗
# ║                        File and Directory Settings                        ║
# ╚═══════════════════════════════════════════════════════════════════════════╝

REGISTRY := ztrldgit01.ztr.biz:7443/next-gen/tools/goat/ui
TAGGED_DOCKER_CONTAINER := $(REGISTRY)/$(PROJECT_NAME):$(VERSION)

ifeq ($(VERBOSE),1)
    DOCKER_VERBOSE_FLAG :=
else
    DOCKER_VERBOSE_FLAG := -q
endif

DOCKER_FILE := ZTR.M7Config.Web/Dockerfile

# ╔═══════════════════════════════════════════════════════════════════════════╗
# ║                                  TARGETS                                  ║
# ╚═══════════════════════════════════════════════════════════════════════════╝

# Note that you do NOT need to specify a `help` or `clean` target as these are
# handled automatically by the top level Makefile.

.PHONY: build push

build:
	@$(INFO) "Building $(TAGGED_DOCKER_CONTAINER) ..."
	$(SILENCER)docker build $(DOCKER_VERBOSE_FLAG) -t $(TAGGED_DOCKER_CONTAINER) -f $(DOCKER_FILE) $(CURDIR)

push: build
	@$(INFO) "Pushing to $(CI_REGISTRY) ..."
	$(SILENCER)docker login -u gitlab-ci-token -p '$(CI_JOB_TOKEN)' '$(CI_REGISTRY)'
	$(SILENCER)docker push '$(TAGGED_DOCKER_CONTAINER)'
