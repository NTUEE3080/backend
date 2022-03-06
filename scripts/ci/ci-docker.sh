#! /bin/sh

# check for necessary env vars
[ "${DOMAIN}" = '' ] && echo "'DOMAIN' env var not set" && exit 1
[ "${GITHUB_REPO_REF}" = '' ] && echo "'GITHUB_REPO_REF' env var not set" && exit 1
[ "${GITHUB_SHA}" = '' ] && echo "'GITHUB_SHA' env var not set" && exit 1
[ "${GITHUB_BRANCH}" = '' ] && echo "'GITHUB_BRANCH' env var not set" && exit 1

[ "${CI_DOCKER_IMAGE}" = '' ] && echo "'CI_DOCKER_IMAGE' env var not set" && exit 1
[ "${CI_DOCKER_CONTEXT}" = '' ] && echo "'CI_DOCKER_CONTEXT' env var not set" && exit 1
[ "${CI_DOCKERFILE}" = '' ] && echo "'CI_DOCKERFILE' env var not set" && exit 1

[ "${DOCKER_PASSWORD}" = '' ] && echo "'DOCKER_PASSWORD' env var not set" && exit 1
[ "${DOCKER_USER}" = '' ] && echo "'DOCKER_USER' env var not set" && exit 1

# Login to GitHub Registry
echo "${DOCKER_PASSWORD}" | docker login docker.pkg.github.com -u "${DOCKER_USER}" --password-stdin

# Obtain image
IMAGE_ID="${DOMAIN}/${GITHUB_REPO_REF}/${CI_DOCKER_IMAGE}"
IMAGE_ID=$(echo "${IMAGE_ID}" | tr '[:upper:]' '[:lower:]') # convert to lower case

# obtaining the version
SHA="$(echo "${GITHUB_SHA}" | head -c 6)"
BRANCH="${GITHUB_BRANCH}"
IMAGE_VERSION="${BRANCH}-${SHA}"

# Generate image references
COMMIT_IMAGE_REF="${IMAGE_ID}:${IMAGE_VERSION}"
BRANCH_IMAGE_REF="${IMAGE_ID}:${BRANCH}"
LATEST_IMAGE_REF="${IMAGE_ID}:latest"
CACHE_IMAGE_REF="${IMAGE_ID}:cache"

echo Commit Image Ref: "${COMMIT_IMAGE_REF}"
echo Branch Image Ref: "${BRANCH_IMAGE_REF}"
echo Latest Image Ref: "${LATEST_IMAGE_REF}"
echo Cache Image Ref: "${CACHE_IMAGE_REF}"

# pull cache and tag it as cache
echo Pulling past images as cache...
docker pull "${BRANCH_IMAGE_REF}" || docker pull "${LATEST_IMAGE_REF}" || true
docker tag "${BRANCH_IMAGE_REF}" "${CACHE_IMAGE_REF}" || docker tag "${LATEST_IMAGE_REF}" "${CACHE_IMAGE_REF}" || true

# build image
echo Building Dockerfile...
docker build "${CI_DOCKER_CONTEXT}" -f "${CI_DOCKERFILE}" --tag "${CI_DOCKER_IMAGE}" --cache-from="${CACHE_IMAGE_REF}"

# push commit version
echo Pushing versioned image..
docker tag "${CI_DOCKER_IMAGE}" "${COMMIT_IMAGE_REF}"
docker push "${COMMIT_IMAGE_REF}"

# push branch version
echo Updating branch version...
docker tag "${CI_DOCKER_IMAGE}" "${BRANCH_IMAGE_REF}"
docker push "${BRANCH_IMAGE_REF}"

# push latest
if [ "$BRANCH" = "main" ]; then
	echo Detected branch is '"main"', pushing to latest...
	docker tag "${CI_DOCKER_IMAGE}" "${LATEST_IMAGE_REF}"
	docker push "${LATEST_IMAGE_REF}"
fi

echo "::set-output name=ref_${CI_DOCKER_IMAGE}::${COMMIT_IMAGE_REF}"
