#ifndef __PARTICLE__
#define __PARTICLE__



#define DEFINE_PARTICLES_PROP(particles) StructuredBuffer<Particle> particles;



struct Particle {
    float3 pos;
    float size;
};



#endif
