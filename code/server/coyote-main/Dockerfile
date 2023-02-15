FROM node:16

#install dependencies
COPY package*.json ./

RUN npm install
#build app source
COPY . .

#expose ports
EXPOSE 8081
EXPOSE 3333

#environment vaiables
ENV PORT=8081
ENV websocketPORT=3333
ENV USER_ID=coyote1
ENV USER_PW=coyote00

CMD ["npm", "start"]

