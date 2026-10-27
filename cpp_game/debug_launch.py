import sys, os, traceback

LOG = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'debug_log.txt')

def log(msg):
    print(msg)
    with open(LOG, 'a', encoding='utf-8') as f:
        f.write(msg + '\n')

# Clear log
open(LOG, 'w').close()
log('=== DEBUG LAUNCH ===')
log(f'Python: {sys.version}')
log(f'CWD: {os.getcwd()}')

try:
    log('Importando pygame...')
    import pygame
    log(f'  pygame OK: {pygame.version.ver}')
except Exception as e:
    log(f'  ERRO pygame: {e}'); input('Pressione Enter...'); sys.exit(1)

try:
    log('Importando OpenGL...')
    from OpenGL.GL import *
    from OpenGL.GLU import *
    log('  OpenGL OK')
except Exception as e:
    log(f'  ERRO OpenGL: {e}'); input('Pressione Enter...'); sys.exit(1)

try:
    log('Inicializando pygame...')
    pygame.init()
    log('  pygame.init OK')

    log('Criando janela 800x600 OpenGL...')
    screen = pygame.display.set_mode((800, 600), pygame.OPENGL | pygame.DOUBLEBUF)
    pygame.display.set_caption('TEST')
    log('  janela OK')

    log('Testando OpenGL básico...')
    vendor   = glGetString(GL_VENDOR)
    renderer = glGetString(GL_RENDERER)
    version  = glGetString(GL_VERSION)
    log(f'  Vendor:   {vendor}')
    log(f'  Renderer: {renderer}')
    log(f'  GL Ver:   {version}')
    max_tex = glGetIntegerv(GL_MAX_TEXTURE_SIZE)
    log(f'  MaxTex:   {max_tex}')

    log('Testando glTexImage2D 512x512...')
    from PIL import Image
    im = Image.new('RGBA', (512, 512), (255,0,0,255))
    tid = glGenTextures(1)
    glBindTexture(GL_TEXTURE_2D, tid)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, 512, 512, 0, GL_RGBA, GL_UNSIGNED_BYTE, im.tobytes())
    log('  glTexImage2D 512x512 OK')

    log('Abrindo viewer3d...')
    pygame.quit()
    import viewer3d
    viewer3d.main()

except Exception as e:
    log(f'ERRO: {e}')
    log(traceback.format_exc())
    input('Pressione Enter para fechar...')
