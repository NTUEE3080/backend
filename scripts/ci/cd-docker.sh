#! /bin/sh

# check for necessary env vars
[ "${DOMAIN}" = '' ] && echo "'DOMAIN' env var not set" && exit 1
[ "${GITHUB_REPO_REF}" = '' ] && echo "'GITHUB_REPO_REF' env var not set" && exit 1
[ "${GITHUB_TAG}" = '' ] && echo "'GITHUB_TAG' env var not set" && exit 1

[ "${CI_DOCKER_IMAGE}" = '' ] && echo "'CI_DOCKER_IMAGE' env var not set" && exit 1
[ "${CI_DOCKER_CONTEXT}" = '' ] && echo "'CI_DOCKER_CONTEXT' env var not set" && exit 1
[ "${CI_DOCKERFILE}" = '' ] && echo "'CI_DOCKERFILE' env var not set" && exit 1

[ "${EXTERNAL_IMAGE_NAME}" = '' ] && echo "'EXTERNAL_IMAGE_NAME' env var not set" && exit 1

[ "${GITHUB_DOCKER_PASSWORD}" = '' ] && echo "'GITHUB_DOCKER_PASSWORD' env var not set" && exit 1
[ "${GITHUB_DOCKER_USER}" = '' ] && echo "'GITHUB_DOCKER_USER' env var not set" && exit 1
[ "${EXTERNAL_DOCKER_PASSWORD}" = '' ] && echo "'EXTERNAL_DOCKER_PASSWORD' env var not set" && exit 1
[ "${EXTERNAL_DOCKER_USER}" = '' ] && echo "'EXTERNAL_DOCKER_USER' env var not set" && exit 1

# Login to GitHub Registry
echo "${GITHUB_DOCKER_PASSWORD}" | docker login docker.pkg.github.com -u "${GITHUB_DOCKER_USER}" --password-stdin

# Obtain image
IMAGE_ID="${DOMAIN}/${GITHUB_REPO_REF}/${CI_DOCKER_IMAGE}"
IMAGE_ID=$(echo "${IMAGE_ID}" | tr '[:upper:]' '[:lower:]') # convert to lower case

# Fix Tag
# shellcheck disable=SC2016
GITHUB_TAG="$(echo "${GITHUB_TAG}" | sd '^.*/v([^/]*)$' '$1')"

# Generate image references
LATEST_IMAGE_REF="${IMAGE_ID}:latest"
CACHED_IMAGE_REF="${IMAGE_ID}:cache"
TAGGED_IMAGE_REF="${IMAGE_ID}:${GITHUB_TAG}"

EXTERNAL_TAGGED_IMAGE_REF="${EXTERNAL_IMAGE_NAME}:${GITHUB_TAG}"
EXTERNAL_LATEST_IMAGE_REF="${EXTERNAL_IMAGE_NAME}:latest"

# Print out reference for debug
echo Latest Image Ref: "${LATEST_IMAGE_REF}"
echo Cached Image Ref: "${CACHED_IMAGE_REF}"
echo Tagged Image Ref: "${TAGGED_IMAGE_REF}"
echo
echo ==============================
echo
echo External Tagged Image Ref: "${EXTERNAL_TAGGED_IMAGE_REF}"
echo External Latest Image Ref: "${EXTERNAL_LATEST_IMAGE_REF}"

# pull cache and tag it as cache
echo Pulling latest image as cache...
docker pull "${LATEST_IMAGE_REF}" || true
docker tag "${LATEST_IMAGE_REF}" "${CACHED_IMAGE_REF}" || true

# build image
echo Building image...
docker build "${CI_DOCKER_CONTEXT}" -f "${CI_DOCKERFILE}" --tag "${CI_DOCKER_IMAGE}" --cache-from="${CACHED_IMAGE_REF}"

# tag built images
echo Tagging image with external references...
docker tag "${CI_DOCKER_IMAGE}" "${TAGGED_IMAGE_REF}"
docker tag "${CI_DOCKER_IMAGE}" "${EXTERNAL_TAGGED_IMAGE_REF}"
docker tag "${CI_DOCKER_IMAGE}" "${EXTERNAL_LATEST_IMAGE_REF}"

# docker push to Github Registry
echo Updating tag...
docker push "${TAGGED_IMAGE_REF}"

# Login to External registry
echo Logging into external Docker Repository...
echo "${EXTERNAL_DOCKER_PASSWORD}" | docker login -u "${EXTERNAL_DOCKER_USER}" --password-stdin

# docker push to external repository
echo Pushing to external repository...
docker push "${EXTERNAL_TAGGED_IMAGE_REF}"
docker push "${EXTERNAL_LATEST_IMAGE_REF}"

echo "::set-output name=ref_${CI_DOCKER_IMAGE}::${EXTERNAL_TAGGED_IMAGE_REF}"
