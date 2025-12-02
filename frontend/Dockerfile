# Stage 1: Build
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package.json from frontend
COPY package*.json ./
RUN npm ci

# Copy all frontend sources (including nuxt.config and app/)
COPY . .

# Build Nuxt for production
RUN npm run build

# Stage 2: Runtime
FROM node:20-alpine AS runner

WORKDIR /app
ENV NODE_ENV=production

# Copy built output and package.json
COPY --from=builder /app/.output ./.output
COPY --from=builder /app/package*.json ./

# Install only production dependencies
RUN npm ci --omit=dev

EXPOSE 3000
CMD ["node", ".output/server/index.mjs"]
