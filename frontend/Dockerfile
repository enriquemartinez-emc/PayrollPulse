FROM node:20-slim

WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy source
COPY . .

# Expose Nuxt default port
EXPOSE 3000

# Run in dev mode (hot reload)
CMD ["npm", "run", "dev"]
